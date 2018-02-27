using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Loader;
using Grpc.Core;
using Demo.Services.Grpc;
using netGRPCServer.tools;
using netGRPCServer.api;
using System.Threading;

namespace netGRPCServer
{
    class Program
    {
		private static bool showExceptionDetails = true;
		private static bool exitSignal = false;
		private static Server server;
		private static DemoManagmentServer demoService;

		public static ServiceConfig serviceConfig;

        static void Main(string[] args)
        {
			AssemblyLoadContext.Default.Unloading += SigTermHandler;
			serviceConfig = new ConfigParser().GetCfg();
			if (serviceConfig.GrpcAddress != string.Empty)
			{
				List<ChannelOption> options = new List<ChannelOption>();
				options.Add(new ChannelOption(ChannelOptions.MaxConcurrentStreams, serviceConfig.MaxConcurrentStreams));
				options.Add(new ChannelOption(ChannelOptions.MaxReceiveMessageLength, serviceConfig.MaxReceiveMessageLength * 1024 * 1024));
				options.Add(new ChannelOption(ChannelOptions.MaxSendMessageLength, serviceConfig.MaxSendMessageLength * 1024 * 1024));
				demoService = new DemoManagmentServer();
				server = new Server(options)
				{
					Services = { DemoManagement.BindService(demoService) },
					Ports = { new ServerPort(serviceConfig.GrpcAddress, serviceConfig.GrpcPort, ServerCredentials.Insecure) }
				};
				server.Start();
				Console.WriteLine($"DemoDataProvider listening on {serviceConfig.GrpcAddress}:{serviceConfig.GrpcPort}");
				Console.WriteLine("Press Ctrl+C to stop the server...");
				Console.CancelKeyPress += Console_CancelKeyPress;
				while (true)
				{
					if (exitSignal) break;
					demoService.dbProvider.CheckDBConnState();
					Thread.Sleep(1000);
				}
			}

        }

		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			exitSignal = true;
		}

		private async static void SigTermHandler(AssemblyLoadContext assemblyLoadContext)
		{
			await server.ShutdownAsync();
			Console.WriteLine("SIGTERM Event.");
		}

		public static void ThrowException(Exception ex, string message = "Error")
		{
			if (showExceptionDetails)
			{
				// Get stack trace for the exception with source file information
				var trace = new StackTrace(ex, true);
				// Get the top stack frame
				var frame = trace.GetFrame(0);
				// Get the line number from the stack frame
				var line = frame.GetFileLineNumber();
				var fileName = frame.GetFileName();
				var method = frame.GetMethod();
				Console.WriteLine($"{message}: '{ex.Message}'. File: '{fileName}'. Method: {method.Name}. Line: {line}. Stack trace: {ex.StackTrace}");
			}
			else
			{
				Console.WriteLine($"{message}: '{ex.Message}'.");
			}
		}
    }
}
