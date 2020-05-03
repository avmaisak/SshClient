
using Microsoft.Extensions.DependencyInjection;
using SshClient.Host.Services;

namespace SshClient.Host.Infrastructure
{
	public static class ServiceConfiguration
	{
		public static IServiceCollection ConfigureServices(IServiceCollection serviceCollection, string basePath)
		{
			serviceCollection.AddJsonConfiguration(basePath);
			serviceCollection.AddTransient<SshPortForwardingService>();

			return serviceCollection;
		}
	}
}
