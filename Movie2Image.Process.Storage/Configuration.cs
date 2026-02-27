using Microsoft.Extensions.DependencyInjection;
using Movie2Image.Process.Application.Ports.Output.Storage;
using Movie2Image.Process.Storage.Basic;

namespace Movie2Image.Process.Storage;

public static class Configuration
{

	public static IServiceCollection AddStorage(this IServiceCollection services)
	{
		services.AddScoped<IStorage, FileSystemStorage>();

		return services;
	}

}
