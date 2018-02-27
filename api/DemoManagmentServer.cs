using System.Threading.Tasks;
using Demo.Services.Grpc;
using Grpc.Core;
using Void = Demo.Services.Grpc.Void;

namespace netGRPCServer.api
{
	public class DemoManagmentServer: DemoManagement.DemoManagementBase
	{
		private DbManagment _dbProvider = new DbManagment();
		public DbManagment dbProvider
		{
			get { return this._dbProvider; }
			set { this._dbProvider = value; }
		}

		public override Task<DemoRecordMetadataSequence> GetRecordMetadataList(GetDemoRecordsMetadataListRequest request, ServerCallContext context)
		{
			return base.GetRecordMetadataList(request, context);
		}

		public override Task<Void> InsertDemoRecord(DemoRecord request, ServerCallContext context)
		{
			this._dbProvider.CreateDemoRecord(request);
			return Task.FromResult(new Void());
		}

		public override Task<Void> InsertInterim(InsertInterimRequest request, ServerCallContext context)
		{
			this._dbProvider.TryCreatetInterim(request);
			return Task.FromResult(new Void());
		}

		public override Task<DemoRecord> LookupNextDemoRecord(LookupNextDemoRecordRequest request, ServerCallContext context)
		{
			var record = this._dbProvider.LookupNextDemoRecord(request.UserId,request.LookupLang);
			return Task.FromResult(record);
		}

		public override Task<Void> RemoveLock(RemoveLockRequest request, ServerCallContext context)
		{
			this._dbProvider.RemoveLock(request.UserId,request.RecordId);
			return Task.FromResult(new Void());
		}

		public override Task<MetadataContent> GetMetadataContent(GetMetadataContentRequest request, ServerCallContext context){
			var metadata = this._dbProvider.GetMetadataContent(request);
			return Task.FromResult(metadata);
		}
	}
}
