using Movie2Image.Process.Application.Ports.Input;
using Movie2Image.Process.Application.Ports.Output.Mail;
using System.Net;
using System.Net.Mail;

namespace Movie2Image.Process.Mail.NetMail;

public class MailSenderService(
	IProcessConfiguration config) : IMailSenderService
{

	public async Task Send(string to, string subject, string body)
	{
		using var client = new SmtpClient(config.MailHost, config.MailPort)
		{
			Credentials = new NetworkCredential(config.MailUsername, config.MailPassword)
		};

		await client.SendMailAsync(config.MailFrom, to, subject, body);
	}

}

