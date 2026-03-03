using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.Ports.Output.Mail;
using System.Net;
using System.Net.Mail;

namespace Movie2Image.Process.Mail.NetMail;

public class MailSenderService(
	IConfiguration config) : IMailSenderService
{

	private readonly string host = config["MAIL_HOST"] 
		?? throw new ArgumentNullException("MAIL_HOST");

	private readonly int port = int.Parse(config["MAIL_PORT"] ?? "25");

	private readonly string mailFrom = config["MAIL_FROM"]
		?? throw new ArgumentNullException("MAIL_FROM");

	private readonly string username = config["MAIL_USERNAME"]
		?? throw new ArgumentNullException("MAIL_USERNAME");

	private readonly string password = config["MAIL_PASSWORD"]
		?? throw new ArgumentNullException("MAIL_PASSWORD");


	public async Task Send(string to, string subject, string body)
	{
		using var client = new SmtpClient(host, port)
		{
			Credentials = new NetworkCredential(username, password)
		};

		await client.SendMailAsync(mailFrom, to, subject, body);
	}

}

