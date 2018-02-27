using System;
using Newtonsoft.Json;

namespace netGRPCServer.tools
{
	public class ConfigParser
	{
		private string path = "server.cfg";

		public ServiceConfig GetCfg()
		{
			if (System.IO.File.Exists(path))
			{
				try
				{
					var jsonText = System.IO.File.ReadAllText(path);
					var config = (ServiceConfig)JsonConvert.DeserializeObject(jsonText, typeof(ServiceConfig));
					return config;
				}
				catch (Exception ex)
				{
					Program.ThrowException(ex, "Failed to parse config file");
					return new ServiceConfig();
				}
			}
			else
			{
				Console.WriteLine($"Config file does not exist!");
				return new ServiceConfig();
			}
		}
	}

	public struct ServiceConfig
	{
		public string GrpcAddress;

		public int GrpcPort;

		public int MaxConcurrentStreams;

		public int MaxReceiveMessageLength;

		public int MaxSendMessageLength;

		public string PostgresAddress;

		public int PostgresPort;

		public string PostgresUser;

		public string PostgresPass;

		public string DemoDb;
	}
}
