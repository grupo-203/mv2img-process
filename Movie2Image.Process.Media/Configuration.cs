using Microsoft.Extensions.DependencyInjection;
using Movie2Image.Process.Application.Ports.Output.Media;
using Movie2Image.Process.Media.FFMPEG;

namespace Movie2Image.Process.Media;

public static class Configuration
{

	public static IServiceCollection AddMedia(this IServiceCollection services)
	{
		services.AddSingleton<IMovieIntoImagesTransform, FfmpegMovieIntoImagesTransform>();

		return services;
	}

}
