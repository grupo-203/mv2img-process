using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Movie2Image.Process.Logging;

public static class Configuration
{

	public static IServiceCollection AddConsoleLogging(this IServiceCollection services)
	{
		ConsoleLogger.LogInformation("Add [Logging]");

		services.AddLogging(builder =>
		{
			builder.AddConsoleFormatter<ConsoleLogFormatter, ConsoleFormatterOptions>();
			builder.AddConsole(options =>
			{
				options.FormatterName = nameof(ConsoleLogFormatter);
			});
		});

		services.AddSingleton(provider =>
		{
			return provider
				.GetRequiredService<ILoggerFactory>()
				.CreateLogger("default");
		});

		return services;
	}


}
