using Microsoft.Extensions.DependencyInjection;
using Movie2Image.Process.Application.Ports.Input.Controller;

namespace Movie2Image.Process.Controller;

public static class Configuration
{

	public static IServiceCollection AddControllers(this IServiceCollection services)
	{
		services.AddScoped<IExtractFramesController, ExtractFramesController>();
		services.AddScoped<IGenerateZipController, GenerateZipController>();
		services.AddScoped<IProcessMovieController, ProcessMovieController>();
		services.AddScoped<IPublishController, PublishController>();

		return services;
	}

}
