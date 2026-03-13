using Microsoft.Extensions.DependencyInjection;
using Movie2Image.Process.Application.Ports.Output.ExternalServices;
using Movie2Image.Process.ExternalServices.Mock;
using Movie2Image.Process.ExternalServices.Services;

namespace Movie2Image.Process.ExternalServices;

public static class Configuration
{

	public static IServiceCollection AddExternalServices(this IServiceCollection services)
	{
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<ILoadService, LoadMock>();
		services.AddScoped<IDeliveryService, DeliveryService>();

		return services;
	}

}
