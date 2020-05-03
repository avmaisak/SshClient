using System;
using System.IO;
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
		/// Base path.
		/// </summary>
		private static string BasePath => Directory.GetParent(AppContext.BaseDirectory).FullName;
		/// <summary>
		/// Entry point.
		/// </summary>
		/// <returns></returns>
		private static async Task Main()
		{
			_serviceProvider = ServiceConfiguration.ConfigureServices(new ServiceCollection(), BasePath).BuildServiceProvider();

			var sshService = _serviceProvider.GetService<SshPortForwardingService>();

			await sshService.StartServiceAsync();
		}
	}
}