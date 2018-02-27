using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using Demo.Services.Grpc;
using Google.Protobuf;
using Npgsql;
using netGRPCServer.tools;


namespace netGRPCServer.api
{
	public class DbManagment
	{
		private NpgsqlConnection _conn;
		private readonly QueryRepository _queryRepo = new QueryRepository();
		private readonly List<NpgsqlCommand> _transactCommands = new List<NpgsqlCommand>();
		private NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();

		//-------------------------------------------------------------------------------------------------
		public DbManagment()
		{
			this.sb.Host = Program.serviceConfig.PostgresAddress;
			this.sb.Username = Program.serviceConfig.PostgresUser;
			this.sb.Password = Program.serviceConfig.PostgresPass;
			this.sb.Database = Program.serviceConfig.DemoDb;

			this._conn = this.InitConnection(this.sb.ConnectionString);

			Console.WriteLine($"DB connection state:{this._conn.State}");
			try
			{
				_queryRepo.CompilePreparedStatemens(this._conn);
			}
			catch (Exception ex)
			{
				Program.ThrowException(ex, "Failed to compile prepared statements");
			}
		}

		//-------------------------------------------------------------------------------------------------
		private NpgsqlConnection InitConnection(string connString)
		{
			var conn = new NpgsqlConnection(connString);
			try
			{
				conn.Open();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Could not open Db connection: {ex.Message}");
				Program.ThrowException(ex, "Failed to establish DB-connection");
			}

			//return conn.State == ConnectionState.Open ? conn : null;
			return conn;
		}

		//-------------------------------------------------------------------------------------------------
		public void CheckDBConnState()
		{
			if (this._conn.State == ConnectionState.Broken ||
			this._conn.State == ConnectionState.Closed ||
			this._conn.State == ConnectionState.Connecting)
			{
				this._conn = this.InitConnection(this.sb.ConnectionString);
				if (this._conn.State == ConnectionState.Open)
				{
					Console.WriteLine($"DB connection restored at {DateTime.Now.ToString()} succesfully!");
				}
				else
				{
					Console.WriteLine($"Failed to restore DB connection at {DateTime.Now.ToString()}!");
				}
			}
		}

		//-------------------------------------------------------------------------------------------------
		public ConnectionState GetConnectionState()
		{
			return this._conn.State;
		}

		//-------------------------------------------------------------------------------------------------
		public bool CreateDemoRecord(DemoRecord request)
		{
			try {
				var cmd = _queryRepo.InsertRecordQuery;
				cmd.Connection = this._conn;
				cmd.Parameters["stage"].Value = false;
				cmd.Parameters["lang_specs"].Value = request.LangSpecs;
				cmd.Parameters["data"].Value = request.Data.ToByteArray();
				cmd.ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				Program.ThrowException(ex, "Error occured on create Demo record");
				return false;
			}
		}

		//-------------------------------------------------------------------------------------------------
		public DemoRecord LookupNextDemoRecord(long userId, ulong lookupLang) {

			try
			{
				DemoRecord record = new DemoRecord();
				var cmd = _queryRepo.LookUpNextRecordQuery;
				cmd.Connection = this._conn;
				cmd.Parameters["userId"].Value = userId;
				cmd.Parameters["lang_specs"].Value = lookupLang;
				cmd.Parameters["timeSpan"].Value = TimeSpan.FromHours(2).TotalMilliseconds;
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						record = this.TryParseRecord(reader);
					}
				}
				if (record.Id != 0)
				{
					bool wasLocked = this.TryLockRecord(record.Id, userId);
					if (!wasLocked)
					{
						return record;
					}
					else
					{
						this.LookupNextDemoRecord(userId, lookupLang);
					}
				}
				else
				{
					// return an empty record
					return record;
				}
			}
			catch (Exception ex)
			{
				Program.ThrowException(ex, "Error occured on lookup next Demo record");
				return null;
			}
			return null;
		}

		//-------------------------------------------------------------------------------------------------
		public bool TryCreatetInterim(InsertInterimRequest request)
		{
			try
			{
				this.ResetTransactionCommands();

				var data = request.Data;
				var userId = request.UserId;
				var recordId = request.RecordId;

				var count = this.CheckInterimsCount(recordId, userId);
				if (count != 0)
				{
					Console.WriteLine("Interim for current record is already exist!");
					return false;
				}
				else
				{
					// Checking interim counts for current record
					var interims = this.GetInterims(recordId);
					if (interims.Count == 1)
					{
						var md5 = MD5.Create();
						var hash = md5.ComputeHash(data.ToByteArray());
						var guid = new Guid(hash);
						// Checking if new interim hash is equal to  exist
						if (interims[0].ChangedDataHash == guid.ToString())
						{
							// Saving confirmed data
							this.TryCommitRecord(userId, recordId, data);
						}
						else
						{
							this.NewInterim(recordId, userId, data.ToByteArray());
						}
					}
					else if (interims.Count == 0)
					{
						this.NewInterim(recordId, userId, data.ToByteArray());
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Program.ThrowException(ex, "Error occured on create interim");
				return false;
			}
		}

		//-------------------------------------------------------------------------------------------------
		public bool RemoveLock(long userId = -1, long recordId = -1, bool useTransaction = false)
		{
			try
			{
				var _lock = this.GeRecordLock(userId, recordId);
				if (_lock.lockId != 0)
				{
					NpgsqlCommand cmd = this._queryRepo.RemoveLockQuery;
					cmd.Connection = this._conn;
					cmd.Parameters["lockId"].Value = _lock.lockId;

					if (!useTransaction)
					{
						cmd.ExecuteNonQuery();
					}
					else
					{
						this._transactCommands.Add(cmd);
					}
					return true;
				}
				else
				{
					Console.WriteLine($"Received empty lock record.");
					return false;
				}
			}
			catch (Exception e)
			{
				Program.ThrowException(e, "Error occured on remove lock");
				return false;
			}
		}

		//-------------------------------------------------------------------------------------------------
		public DemoRecordMetadataSequence GetDemoRecordsMetadataList(GetDemoRecordsMetadataListRequest request) {
			try
			{
				DemoRecordMetadataSequence metaList = new DemoRecordMetadataSequence();
				NpgsqlCommand cmd = this._queryRepo.GetRecordsHeaders;
				cmd.Connection = this._conn;
				cmd.Parameters["skip"].Value = request.Skip;
				cmd.Parameters["limit"].Value = request.Limit;

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var record = this.TryParseRecord(reader);
						metaList.DemoRecordMetadataList.Add(new DemoRecordMetadata() {
							Id = record.Id,
							Stage = record.Stage,
							LangSpecs = record.LangSpecs,
							Title = record.Title,

						});
					}
				}
			}
			catch (Exception e)
			{
				Program.ThrowException(e, "Error occured on receive metadata list");
			}

			return null;
		}

		//-------------------------------------------------------------------------------------------------
		public MetadataContent GetMetadataContent(GetMetadataContentRequest request) {
			MetadataContent content = new MetadataContent();
			try
			{
				var record = this.GetDemoRecord(request.RecordId);
				var interims = this.GetInterims(request.RecordId);
				content.Data = record.Stage ? record.ApprovedData : record.Data;
				foreach(var item in interims) {
					content.Interims.Add(item.ChangedData);	
				}
			}
			catch (Exception e)
			{
				Program.ThrowException(e, "Error occured on receive metadata content");
			}
			return content;

		}

		//-------------------------------------------------------------------------------------------------
		private DemoRecord TryParseRecord(NpgsqlDataReader reader) {
			try
			{
				var record_id = (long)reader["record_id"];
				bool stage = false;
				bool.TryParse(reader["stage"].ToString(), out stage);
				DateTime creation_time = DateTime.Now;
				DateTime.TryParse(reader["creation_time"].ToString(), out creation_time);
				var dataBytes = reader.GetOrdinal("data") >= 0 ? reader["data"] : null;
				var approved_dataBytes = reader.GetOrdinal("approved_data") >= 0 ? reader["approved_data"] : null;
				var lang_specs = (ulong)reader["lang_specs"];
				var title = (string)reader["title"];

				// Create Demo record
				DemoRecord record = new DemoRecord();
				record.Id = record_id;
				record.CreationTimestamp = this.DateTimeToMs(creation_time);
				record.Stage = stage;
				record.LangSpecs = lang_specs;
				record.Title = title;
				if (dataBytes != null)
				{
					record.Data = DemoContent.Parser.ParseFrom(dataBytes as byte[]);
				}
				if (approved_dataBytes != null)
				{
					record.ApprovedData = DemoContent.Parser.ParseFrom(approved_dataBytes as byte[]);
				}
				return record;
			}
			catch (Exception ex)
			{
				Program.ThrowException(ex, "Error occured on parse DemoRecord");
				return null;
			}
		}

		//-------------------------------------------------------------------------------------------------
		private Interim TryParseInterim(NpgsqlDataReader reader)
		{
			try
			{
				Interim interim = new Interim();
				interim.InterimId = (long)reader["interim_id"];
				interim.RecordId = (long)reader["record_id"];
				interim.CreatedBy = (long)reader["created_by"];
				var time = (DateTime)(reader["creation_time"]);
				interim.CreationTimestamp = time.Ticks;
				interim.ChangedData = DemoContent.Parser.ParseFrom(reader["changed_data"] as byte[]);
				interim.ChangedDataHash = reader["changed_data_hash"].ToString();//(Guid)
				return interim;
			}
			catch (Exception ex)
			{
				Program.ThrowException(ex, "Error occured on parse interim");
				return null;
			}
		}

		//-------------------------------------------------------------------------------------------------
		private bool TryCommitRecord(long userId,long recordId, DemoContent data) {
			this.RemoveLock(userId, recordId, true);
			var demoRecord = this.GetDemoRecord(recordId);
			if (demoRecord.Id != 0)
			{
				// Create changed data hash
				var md5 = MD5.Create();
				var hash = md5.ComputeHash(data.ToByteArray());
				var guid = new Guid(hash);

				this.ApproveRecord(recordId, data, true);
				var interims = this.GetInterims(recordId);

				interims.ForEach((Interim interim) =>
				{
					this.UpdateInterim(
						interim.InterimId,
						data.ToByteArray(),
						guid,
						true
					);
				});

				// Creating admin interim
				this.InsertInterim(
						recordId,
						userId,
						data.ToByteArray(),
						guid,
						true
					);
				return true;
			}
			else
			{
				Console.WriteLine("Record not found!");
				return false;
			}
		}

		//-------------------------------------------------------------------------------------------------
		private long DateTimeToMs(DateTime value) {
			return (long)value.ToUniversalTime().Subtract(
				new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
			).TotalMilliseconds;
		}

		//-------------------------------------------------------------------------------------------------
		private bool CheckLockStatus(long userId, ref long recordId)
		{
			this.ResetTransactionCommands();
			//At First checking active lock for user
			var userLock = this.GeRecordLock(userId, -1);
			var recordLock = this.GeRecordLock(-1, recordId);
			// В поточного користувача є лок на запис
			if (userLock != null)
			{
				// Лок на поточний запис
				if (userLock.recordId == recordId)
				{
					// Час локу вийшов
					if ((DateTime.UtcNow.Ticks - userLock.lockTime) > TimeSpan.FromHours(2).Ticks)
					{
						// Remove outdate Lock 
						this.RemoveLock(userId, -1, true);
						this.LockRecord(userId, recordId, true);
						this.RunTransaction();
						return false;
					}
					// Час локу не вийшов
					else
					{
						return false;
					}
				}
				//Лок на інший запис
				else
				{
					// Час локу вийшов
					if ((DateTime.UtcNow.Ticks - userLock.lockTime) > TimeSpan.FromHours(2).Ticks)
					{
						this.RemoveLock(userId, -1, true);
						this.LockRecord(userId, recordId, true);
						this.RunTransaction();
						return false;
					}
					// Час локу не вийшов
					else
					{
						recordId = userLock.recordId;
						return true;
					}
				}
			}
			// В поточного користувача немає локу
			else
			{
				// В поточного запису нема локу 
				if (recordLock == null)
				{
					this.LockRecord(userId, recordId, true);
					this.RunTransaction();
					return false;
				}
				// В поточного запису є лок
				else
				{
					// Час локу вийшов
					if ((DateTime.UtcNow.Ticks - recordLock.lockTime) > TimeSpan.FromHours(2).Ticks)
					{
						this.RemoveLock(userId, -1, true);
						this.LockRecord(userId, recordId, true);
						this.RunTransaction();
						return false;
					}
					// Час локу не вийшов
					else
					{
						return true;
					}
				}
			}
		}

		//-------------------------------------------------------------------------------------------------
		private bool TryLockRecord(long recordId = -1, long userId = -1)
		{

			if (recordId == -1 || userId == -1)
			{
				return false;
			}
			var prevRecordId = recordId;
			bool IsLocked = this.CheckLockStatus(userId, ref prevRecordId);
			return IsLocked;
		}

		//-------------------------------------------------------------------------------------------------
		private Lock GeRecordLock(long userId = -1, long recordId = -1)
		{
			NpgsqlCommand cmd = new NpgsqlCommand();
			Lock _lock = new Lock();
			if (recordId != -1 && userId != -1)
			{
				cmd = this._queryRepo.GetLockQuery;
				cmd.Connection = this._conn;
				cmd.Parameters["userId"].Value = userId;
				cmd.Parameters["recordId"].Value = recordId;
			}
			else if (recordId != -1 && userId == -1)
			{
				cmd = this._queryRepo.GetRecordLockQuery;
				cmd.Connection = this._conn;
				cmd.Parameters["recordId"].Value = recordId;
			}
			else if (recordId == -1 && userId != -1)
			{
				cmd = this._queryRepo.GetUserLockQuery;
				cmd.Connection = this._conn;
				cmd.Parameters["userId"].Value = userId;
			}

			using (var reader = cmd.ExecuteReader())
			{
				if (reader.HasRows)
				{
					while (reader.Read())
					{
						_lock.lockId = (long)reader["lock_id"];
						_lock.recordId = (long)reader["record_id"];
						_lock.lockedBy = (long)reader["locked_by"];
						var time = (DateTime)(reader["lock_time"]);
						_lock.lockTime = time.Ticks;
					}
					return _lock;
				}
				else return null;
			}
		}

		//-------------------------------------------------------------------------------------------------
		private int LockRecord(long userId, long recordId, bool useTransaction = false)
		{
			try
			{
				NpgsqlCommand cmd = this._queryRepo.InsertLockQuery;
				cmd.Connection = this._conn;
				cmd.Parameters["userId"].Value = userId;
				cmd.Parameters["recordId"].Value = recordId;
				if (!useTransaction)
				{
					cmd.ExecuteNonQuery();
				}
				else
				{
					this._transactCommands.Add(cmd);
				}
				return 0;
			}
			catch (Exception ex)
			{
				Program.ThrowException(ex, "Error occured on locking record");
				return -1;
			}
		}

		//-------------------------------------------------------------------------------------------------
		private int CheckInterimsCount(long recordId, long userId)
		{
			NpgsqlCommand cmd = this._queryRepo.InterimsCountQuery;
			cmd.Connection = this._conn;
			cmd.Parameters["recordId"].Value = recordId;
			cmd.Parameters["userId"].Value = userId;
			int count = 0;

			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					var result = int.TryParse(reader[0].ToString(), out count);
				}
			}

			return count;
		}

		//-------------------------------------------------------------------------------------------------
		private List<Interim> GetInterims(long recordId)
		{
			NpgsqlCommand cmd = this._queryRepo.GetInterimsQuery;
			cmd.Connection = this._conn;
			cmd.Parameters["recordId"].Value = recordId;
			List<Interim> interims = new List<Interim>();
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					interims.Add(this.TryParseInterim(reader));
				}
			}
			return interims;
		}

		//-------------------------------------------------------------------------------------------------
		private void ResetTransactionCommands()
		{
			this._transactCommands.Clear();
		}

		//-------------------------------------------------------------------------------------------------
		private void RunTransaction()
		{
			try
			{

				using (var transaction = this._conn.BeginTransaction())
				{
					foreach (var command in this._transactCommands)
					{
						command.ExecuteNonQuery();
					}
					transaction.Commit();
				}
			}
			catch (Exception e)
			{
				Debug.Assert(false, e.Message);
			}
		}

		//-------------------------------------------------------------------------------------------------
		private void NewInterim(long recordlId, long userId, byte[] data)
		{
			this.ResetTransactionCommands();
			var md5 = MD5.Create();
			var hash = md5.ComputeHash(data);
			var guid = new Guid(hash);
			this.InsertInterim(recordlId, userId, data, guid, true);

			this.RemoveLock(userId, recordlId, true);
		}

		//-------------------------------------------------------------------------------------------------
		private DemoRecord GetDemoRecord(long recordId)
		{
			NpgsqlCommand cmd = this._queryRepo.GetRecordQuery;
			cmd.Connection = this._conn;
			cmd.Parameters["recordId"].Value = recordId;
			DemoRecord record = new DemoRecord();
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					record = this.TryParseRecord(reader);
				}
			}
			return record;
		}


		//-------------------------------------------------------------------------------------------------
		private bool UpdateInterim(
				long interimId,
				byte[] changedData,
				Guid guid,
				bool useTransaction = false)
		{
			try
			{
				NpgsqlCommand cmd = this._queryRepo.UpdateInterimQuery;
				cmd.Connection = this._conn;
				cmd.Parameters["id"].Value = interimId;
				cmd.Parameters["chDta"].Value = changedData;
				cmd.Parameters["hash"].Value = guid;
				if (!useTransaction)
				{
					cmd.ExecuteNonQuery();
				}
				else
				{
					this._transactCommands.Add(cmd);
				}
				return true;
			}
			catch (Exception ex)
			{
				Program.ThrowException(ex, "Error occured on updating interim");
				return false;
			}
		}


		//-------------------------------------------------------------------------------------------------
		private long GetRecordsCount(long recordId)
		{
			NpgsqlCommand cmd = this._queryRepo.RecordsCountQuery;
			cmd.Connection = this._conn;
			cmd.Parameters["recordId"].Value = recordId;
			long count = 0;
			using (var reader = cmd.ExecuteReader())
			{
				while (reader.Read())
				{
					count = (long)reader["count"];
				}
			}
			return count;
		}

		//-------------------------------------------------------------------------------------------------
		private bool InsertInterim(
				long recordId,
				long userId,
				byte[] changedData,
				Guid guid,
				bool useTransaction = false
			)
		{
			try
			{
				// Creating user interim
				NpgsqlCommand cmd = this._queryRepo.InsertInterimQuery;
				cmd.Connection = this._conn;
				cmd.Parameters["recordId"].Value = recordId;
				cmd.Parameters["userId"].Value = userId;
				cmd.Parameters["chData"].Value = changedData;
				cmd.Parameters["hash"].Value = guid;
				if (!useTransaction)
				{
					cmd.ExecuteNonQuery();
				}
				else
				{
					this._transactCommands.Add(cmd);
				}
				return true;
			}
			catch (Exception e)
			{
				Program.ThrowException(e, "Error occured on insert new interim");
				return false;
			}
		}

		//-------------------------------------------------------------------------------------------------
		private bool ApproveRecord(long recordId,DemoContent data, bool useTransaction = false)
		{
			try
			{
				NpgsqlCommand cmd = this._queryRepo.ApproveRecordQuery;

				cmd.Connection = this._conn;
				cmd.Parameters["recordId"].Value = recordId;
				cmd.Parameters["stage"].Value = true;
				cmd.Parameters["approved_data"].Value = data.ToByteArray();
				if (!useTransaction)
				{
					cmd.ExecuteNonQuery();
				}
				else
				{
					this._transactCommands.Add(cmd);
				}
				return true;
			}
			catch (Exception e)
			{
				Program.ThrowException(e, "Error occured on commiting record");
				return false;
			}
		}
	}

	class Lock	{
		public long lockId { set; get; }
		public long recordId { set; get; }
		public long lockedBy { set; get; }
		public long lockTime { set; get; }
	}
}
