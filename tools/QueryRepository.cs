using Npgsql;
using NpgsqlTypes;

namespace netGRPCServer.tools
{
	public class QueryRepository
	{
		/* DemoRecord queries */
		// I/O queries
		public NpgsqlCommand InsertRecordQuery { set; get; }
		public NpgsqlCommand DeletRecordQuery { set; get; }
		public NpgsqlCommand GetRecordQuery { set; get; }
		public NpgsqlCommand LookUpNextRecordQuery { set; get; }
		public NpgsqlCommand RecordsCountQuery { set; get; }
		// Managment queries
		public NpgsqlCommand ApproveRecordQuery { set; get; }

		/* Interims queries */
		// I/O queries
		public NpgsqlCommand InsertInterimQuery { set; get; }
		public NpgsqlCommand GetInterimsQuery { set; get; }
		public NpgsqlCommand GetRecordsHeaders { set; get; }

		// Managment queries
		public NpgsqlCommand InterimsCountQuery { set; get; }
		public NpgsqlCommand UpdateInterimQuery { set; get; }

		/* Locks queries */
		public NpgsqlCommand InsertLockQuery { set; get; }
		public NpgsqlCommand RemoveLockQuery { set; get; }

		public NpgsqlCommand GetLockQuery { set; get; }
		public NpgsqlCommand GetRecordLockQuery { set; get; }
		public NpgsqlCommand GetUserLockQuery { set; get; }


		public QueryRepository() { }

		public void CompilePreparedStatemens(NpgsqlConnection conn)
		{
			/* DemoRecord Prepared Statement */
			// I/O queries
			this.InsertRecordQuery = new NpgsqlCommand("INSERT INTO demo_records(" +
			                                           " stage,data,creation_time,lang_specs)" +
			                                           " VALUES (@stage, @data, CURRENT_TIMESTAMP,@lang_specs);", conn);
			this.InsertRecordQuery.Parameters.Add("stage", NpgsqlDbType.Boolean);
			this.InsertRecordQuery.Parameters.Add("data", NpgsqlDbType.Bytea);
			this.InsertRecordQuery.Parameters.Add("lang_specs",NpgsqlDbType.Integer);
			this.InsertRecordQuery.Prepare();

			this.DeletRecordQuery = new NpgsqlCommand("DELETE FROM locks WHERE record_id=@recordId", conn);
			this.DeletRecordQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.DeletRecordQuery.Prepare();

			this.GetRecordQuery = new NpgsqlCommand("SELECT * FROM demo_records WHERE record_id=@recordId", conn);
			this.GetRecordQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.GetRecordQuery.Prepare();

			this.RecordsCountQuery = new NpgsqlCommand("SELECT count(1) FROM demo_records" +
											" WHERE record_id = @recordId;", conn);
			this.RecordsCountQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.RecordsCountQuery.Prepare();

			this.LookUpNextRecordQuery = new NpgsqlCommand("SELECT * " +
			                                        " FROM demo_records" +
													" LEFT JOIN locks AS lockedItems" +
			                                        " 	ON demo_records.record_id = lockedItems.record_id" +
													" LEFT JOIN interims AS commonitems" +
			                                        " 	ON demo_records.record_id = commonitems.record_id" +
													" WHERE" +
													// Вибірка всіх елементів, які ніколи не вибирались
													" commonitems.record_id IS NULL" +
			                                        " AND demo_records.record_id IS NOT NULL" +
													// Перевірка чи нема локу для поточного запису
			                                        " AND lockedItems.record_id IS NULL" +
			                                        " AND demo_records.record_id IS NOT NULL" +
													//Перевірка чи не вийшов час локу і запит надіслав користувач, для якого вже є лок
													" OR  extract(epoch FROM CURRENT_TIMESTAMP - lockedItems.lock_time)  < @timeSpan" +
													" AND lockedItems.locked_by = @userId" +
													// Додаткова перевірка на елементи, які ніколи не вибирались
			                                        " AND commonitems.record_id IS NULL" +
			                                        " AND demo_records.record_id IS NOT NULL" +
													// Перевірка чи вийшов час локу
													" OR extract(epoch from CURRENT_TIMESTAMP - lockeditems.lock_time) > @timeSpan" +
													" OR commonitems.created_by != @userId" +
													//Перевірка чи кількість interims для одного запису < 2
			                                        " AND  demo_records.record_id IN" +
													" (" +
			                                        " SELECT   record_id" +
													" FROM  interims" +
			                                        " GROUP BY record_id" +
													" HAVING   Count(*) < 2)" +
			                                        " AND lang_specs & @lang_bitmask = 0" +
													// Вибірка всіх елементів, які ніколи не вибирались
													//" AND demo_records.record_id IS NULL" +
													//" AND demo_records.record_id IS NOT NULL" +
			                                        " ORDER BY demo_records.record_id ASC" +
													" LIMIT 1 OFFSET 0;", conn);
			this.LookUpNextRecordQuery.Parameters.Add("userId", NpgsqlDbType.Bigint);
			this.LookUpNextRecordQuery.Parameters.Add("timeSpan", NpgsqlDbType.Integer);
			this.LookUpNextRecordQuery.Parameters.Add("lang_bitmask", NpgsqlDbType.Integer);
			this.LookUpNextRecordQuery.Prepare();

			this.GetRecordsHeaders = new NpgsqlCommand("SELECT t1.record_id, t1.stage," +
			                                         " t1.creation_time, t1.title, t1.lang_specs" +
			                                         " FROM demo_records t1" +
			                                         " INNER JOIN(" +
			                                         " SELECT record_id" +
			                                         " FROM interims" +
			                                         " GROUP BY record_id)" +
			                                         " t2 on t1.record_id = t2.record_id" +
			                                         " ORDER BY t1.record_id ASC" +
													 " LIMIT @limit OFFSET @skip;", conn);
			this.GetRecordsHeaders.Parameters.Add("skip", NpgsqlDbType.Smallint);
			this.GetRecordsHeaders.Parameters.Add("limit", NpgsqlDbType.Smallint);
			this.GetRecordsHeaders.Prepare();

			// Managment queries
			this.ApproveRecordQuery = new NpgsqlCommand("UPDATE demo_records" +
			                                            " SET approved_data= @approved_data,stage=@stage" +
			                                            " WHERE record_id=@recordId;", conn);
			this.ApproveRecordQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.ApproveRecordQuery.Parameters.Add("stage", NpgsqlDbType.Boolean);
			this.ApproveRecordQuery.Parameters.Add("approved_data", NpgsqlDbType.Bytea);
			this.ApproveRecordQuery.Prepare();

			/* Interims Prepared Statement */
			this.InsertInterimQuery = new NpgsqlCommand("INSERT INTO interims(" +
													" record_id,created_by," +
			                                        " creation_time, changed_data, changed_data_hash)" +
													" VALUES (@recordId, @userId, CURRENT_TIMESTAMP," +
			                                            " @chData, @hash);", conn);
			this.InsertInterimQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.InsertInterimQuery.Parameters.Add("userId", NpgsqlDbType.Bigint);
			this.InsertInterimQuery.Parameters.Add("chData", NpgsqlDbType.Bytea);
			this.InsertInterimQuery.Parameters.Add("hash", NpgsqlDbType.Uuid);
			this.InsertInterimQuery.Prepare();

			this.GetInterimsQuery = new NpgsqlCommand("SELECT * FROM interims WHERE record_id=@recordId", conn);
			this.GetInterimsQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.GetInterimsQuery.Prepare();


			this.InterimsCountQuery = new NpgsqlCommand("SELECT count(1) FROM interims" +
												" WHERE record_id = @recordId" +
												" AND created_by=@userId;", conn);
			this.InterimsCountQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.InterimsCountQuery.Parameters.Add("userId", NpgsqlDbType.Bigint);
			this.InterimsCountQuery.Prepare();

			this.UpdateInterimQuery = new NpgsqlCommand("UPDATE interims" +
											" SET changed_data=@chData, changed_data_hash=@hash" +
											" WHERE interim_id=@id;", conn);
			this.UpdateInterimQuery.Parameters.Add("id", NpgsqlDbType.Bigint);
			this.UpdateInterimQuery.Parameters.Add("chData", NpgsqlDbType.Bytea);
			this.UpdateInterimQuery.Parameters.Add("hash", NpgsqlDbType.Uuid);
			this.UpdateInterimQuery.Prepare();


			/* Locks Prepared Statement */
			this.InsertLockQuery = new NpgsqlCommand("INSERT INTO locks(" +
			                                         " record_id, locked_by, lock_time)" +
											   " VALUES (@recordId, @userId, CURRENT_TIMESTAMP);", conn);
			this.InsertLockQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.InsertLockQuery.Parameters.Add("userId", NpgsqlDbType.Bigint);
			this.InsertLockQuery.Prepare();

			this.RemoveLockQuery = new NpgsqlCommand("DELETE FROM locks WHERE lock_id=@lockId", conn);
			this.RemoveLockQuery.Parameters.Add("lockId", NpgsqlDbType.Bigint);
			this.RemoveLockQuery.Prepare();

			this.GetLockQuery = new NpgsqlCommand("SELECT * FROM locks" +
											 " WHERE locked_by=@userId" +
											 " AND record_id=@recordId", conn);
			this.GetLockQuery.Parameters.Add("userId", NpgsqlDbType.Bigint);
			this.GetLockQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.GetLockQuery.Prepare();

			this.GetRecordLockQuery = new NpgsqlCommand("SELECT * FROM locks WHERE record_id=@recordId", conn);
			this.GetRecordLockQuery.Parameters.Add("recordId", NpgsqlDbType.Bigint);
			this.GetRecordLockQuery.Prepare();

			this.GetUserLockQuery = new NpgsqlCommand("SELECT * FROM locks WHERE locked_by=@userId", conn);
			this.GetUserLockQuery.Parameters.Add("userId", NpgsqlDbType.Bigint);
			this.GetUserLockQuery.Prepare();
		}
	}
}
