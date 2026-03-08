using Microsoft.Extensions.DependencyInjection;
using Movie2Image.Process.Application.Ports.Output.Mail;
using Movie2Image.Process.Mail.NetMail;

namespace Movie2Image.Process.Mail;

public static class Configuration
{

	public static IServiceCollection AddMail(this IServiceCollection services)
	{
		services.AddScoped<IMailSenderService, MailSenderService>();

		return services;
	}

}
