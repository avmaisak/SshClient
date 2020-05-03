using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SshClient.Host.Infrastructure
{
	public static class Configuration
	{
		public static IServiceCollection AddJsonConfiguration(this IServiceCollection serviceCollection, string basePath)
		{
			// Build configuration
			var configuration = new ConfigurationBuilder()
					.SetBasePath(basePath)
					.AddJsonFile("appsettings.json", false)
					.Build();
			// Add access to generic IConfigurationRoot
			serviceCollection.AddSingleton<IConfiguration>(configuration);

			return serviceCollection;
		}
	}
}
