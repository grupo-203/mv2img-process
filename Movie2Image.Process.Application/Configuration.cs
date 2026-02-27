using Microsoft.Extensions.DependencyInjection;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Services;
using Movie2Image.Process.Application.UseCases;

namespace Movie2Image.Process.Application;

public static class Configuration
{


	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		return services
			.AddServices()
			.AddUseCases();
	}



	private static IServiceCollection AddServices(this IServiceCollection services)
	{
		services.AddScoped<ITempCleanService, TempCleanService>();
		services.AddScoped<ITempZipPathSetService, TempZipPathSetService>();
		services.AddScoped<IZipPathSetService, ZipPathSetService>();
		services.AddScoped<IFramesPathSetService, FramesPathSetService>();

		return services;
	}

	private static IServiceCollection AddUseCases(this IServiceCollection services)
	{
		services.AddScoped<IExtractFramesUseCase, ExtractFramesUseCase>();
		services.AddScoped<IGenerateZipUseCase, GenerateZipUseCase>();
		services.AddScoped<IPublishUseCase, PublishUseCase>();
		services.AddScoped<IProcessErrorUseCase, ProcessErrorUseCase>();

		return services;
	}

}
