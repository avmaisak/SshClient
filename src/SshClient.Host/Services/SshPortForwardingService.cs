using System;
using System.Threading.Tasks;
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

				var forwardPort = new ForwardedPortLocal(host, port, host, port);

				_client.AddForwardedPort(forwardPort);

				forwardPort.Start();
			}
		}

		public SshPortForwardingService(IConfiguration cfg)
		{
			_cfg = cfg;

			InitConnection();
			InitClient();
		}

		public async Task StartServiceAsync()
		{

			using (_client) {
				await Task.Run(() => _client.Connect());
				InitPortWorwarding();
				Console.ReadKey();
				_client.Disconnect();
			}

		}
	}
}
