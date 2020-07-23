using System;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using SshClient.Host.Infrastructure;
using SshClient.Host.Models;

namespace SshClient.Host.Services
{
	public class SshPortForwardingService
	{
		private readonly IConfiguration _cfg;
		private ConnectionInfo _connectionInfo;
		private Renci.SshNet.SshClient _client;

		/// <summary>
		/// Init connection.
		/// </summary>
		private void InitConnection()
		{
			var connectionModel = _cfg.GetSection("connection").Get<ConnectionModel>();
			if (connectionModel == null) throw new SshClientException(Resources.ConnectionCfgNotSpecified);

			Console.WriteLine($@"Using Connection: {connectionModel.Host}:{connectionModel.Port}");

			var authMethod = new PasswordAuthenticationMethod(connectionModel.UserName, connectionModel.Password);
			_connectionInfo = new ConnectionInfo(connectionModel.Host, connectionModel.Port, connectionModel.UserName, authMethod);
		}

		/// <summary>
		/// Init SSH client.
		/// </summary>
		private void InitClient()
		{
			_client = new Renci.SshNet.SshClient(_connectionInfo)
			{
				KeepAliveInterval = new TimeSpan(0, 0, 30),
				ConnectionInfo = { Timeout = new TimeSpan(0, 0, 20) }
			};
		}
		/// <summary>
		/// Init forwarding ports.
		/// </summary>
		private void InitPortWorwarding()
		{
			var list = _cfg.GetSection("portsForwarding").Get<string[]>();
			if (list?.Length == 0) throw new SshClientException(Resources.ConnectionPortsNotSpecified);

			for (var i = 0; i < list?.Length; i++)
			{
				var cfgArray = list[i].Split(":");
				if (cfgArray.Length < 2) continue;
				
				var host = cfgArray[0];
				var port = Convert.ToUInt32(cfgArray[1]);

				if (string.IsNullOrWhiteSpace(host)) continue;

				Console.WriteLine($@"Forward: {host}: {port}");

				var forwardPort = new ForwardedPortLocal(host, port, host, port);

				_client.AddForwardedPort(forwardPort);

				if(!forwardPort.IsStarted) forwardPort.Start();
			}
		}

		public SshPortForwardingService(IConfiguration cfg)
		{
			_cfg = cfg;

			InitConnection();
			InitClient();
		}

		public void StartService()
		{
			using (_client)
			{
				if (!_client.IsConnected) _client.Connect();

				Console.WriteLine(_client.ConnectionInfo.ClientVersion);

				InitPortWorwarding();
				Console.WriteLine(@"Press any key to shutdown...");
				Console.ReadKey();
				_client.Disconnect();
			}
		}
	}
}
