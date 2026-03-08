namespace Movie2Image.Process.Application.Ports.Input;

public interface IProcessConfiguration
{

    string FramesPath { get; }

    string ZipPath { get; }

    int maxRetries { get; }

    string AuthServiceUrl { get; }

    string LoadServiceUrl { get; }

    string ClientId { get; }

    string ClientSecret { get; }

    string MailHost { get; }

    int MailPort { get; }

    string MailFrom { get; }

    string MailUsername { get; }

    string MailPassword { get; }

    int SecondsPerFrame { get; }

    string RabbitMQConnectionString { get; }

}
