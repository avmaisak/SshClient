using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SshClient.Host.Infrastructure;
using SshClient.Host.Services;

namespace SshClient.Host
{
	/// <summary>
	/// SshClient Host Program.
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// Service Provider.
		/// </summary>
		private static IServiceProvider _serviceProvider;
		/// <summary>
		/// Entry point.
		/// </summary>
		/// <returns></returns>
		private static async Task Main()
		{
			_serviceProvider = ServiceConfiguration.ConfigureServices(new ServiceCollection(), Environment.CurrentDirectory).BuildServiceProvider();

			await Task.Run(() => 
					_serviceProvider.
						GetService<SshPortForwardingService>()
						.StartService()
			);
		}
	}
}