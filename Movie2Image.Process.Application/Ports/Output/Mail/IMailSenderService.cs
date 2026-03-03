namespace Movie2Image.Process.Application.Ports.Output.Mail;

public interface IMailSenderService
{

	public Task Send(string to, string subject, string body);

}
