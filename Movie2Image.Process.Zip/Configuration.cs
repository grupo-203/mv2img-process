using Microsoft.Extensions.DependencyInjection;
using Movie2Image.Process.Application.Ports.Output.Zip;
using Movie2Image.Process.Zip.DotnetZip;

namespace Movie2Image.Process.Zip;

public static class Configuration
{

	public static IServiceCollection AddZip(this IServiceCollection services)
	{
		services.AddSingleton<IZipCreate, DotnetZipFiles>();

		return services;
	}

}
