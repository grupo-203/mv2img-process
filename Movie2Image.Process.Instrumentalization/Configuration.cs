using Microsoft.Extensions.DependencyInjection;

namespace Movie2Image.Process.Instrumentalization;

public static class Configuration
{

	public static IServiceCollection AddInstrumentalization(this IServiceCollection services)
	{
		return services;
	}

}
