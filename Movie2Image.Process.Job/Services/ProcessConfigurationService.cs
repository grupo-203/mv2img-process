using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.Ports.Input;

namespace Movie2Image.Process.Job.Services;

public class ProcessConfigurationService(
    IConfiguration config) : IProcessConfiguration
{

    public string FramesPath => config["FRAMES_PATH"]
        ?? throw new ArgumentNullException("FRAMES_PATH");

    public string ZipPath => config["ZIP_PATH"]
        ?? throw new ArgumentNullException("ZIP_PATH");

    public int maxRetries => Convert.ToInt32(config["ERROR_MAX_RETRIES"]
        ?? throw new ArgumentNullException("ERROR_MAX_RETRIES"));

    public string AuthServiceUrl => config["AUTH_SERVICE_URL"]
        ?? throw new ArgumentNullException("AUTH_SERVICE_URL");

    public string LoadServiceUrl => config["LOAD_SERVICE_URL"]
        ?? throw new ArgumentNullException("LOAD_SERVICE_URL"); 

    public string ClientId => config["CLIENT_ID"]
        ?? throw new ArgumentNullException("CLIENT_ID"); 

    public string ClientSecret => config["CLIENT_SECRET"]
        ?? throw new ArgumentNullException("CLIENT_SECRET");

    public string MailHost => config["MAIL_HOST"]
        ?? throw new ArgumentNullException("MAIL_HOST");

    public int MailPort => int.Parse(config["MAIL_PORT"]
        ?? throw new ArgumentNullException("MAIL_PORT"));

    public string MailFrom => config["MAIL_FROM"]
        ?? throw new ArgumentNullException("MAIL_FROM");

    public string MailUsername => config["MAIL_USERNAME"]
        ?? throw new ArgumentNullException("MAIL_USERNAME");

    public string MailPassword => config["MAIL_PASSWORD"]
        ?? throw new ArgumentNullException("MAIL_PASSWORD");

    public int SecondsPerFrame => int.Parse(config["SECONDS_PER_FRAME"]
        ?? throw new ArgumentNullException("SECONDS_PER_FRAME"));

    public string RabbitMQConnectionString => config["RABBITMQ_CONNECTION"]
        ?? throw new ArgumentNullException("RABBITMQ_CONNECTION");

}
