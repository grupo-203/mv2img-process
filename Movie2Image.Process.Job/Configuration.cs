using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movie2Image.Process.Application.Ports.Input;
using Movie2Image.Process.Job.Services;
using Movie2Image.Process.Logging;

namespace Movie2Image.Process.Job;

public static class Configuration
{
	
	public static IServiceCollection AddConfiguration(this IServiceCollection services)
	{
		ConsoleLogger.LogInformation("Add [Configuration]");

		var builder = new ConfigurationBuilder();

		builder.AddEnvironmentVariables();

		var configuration = builder.Build();
		var processConfig = new ProcessConfigurationService(configuration);

        return services
			.AddSingleton<IConfiguration>(configuration)
			.AddSingleton<IProcessConfiguration>(processConfig);
	}

}
