using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.Ports.Input;

namespace Movie2Image.Process.Application.Test;

public class TestProcessConfiguration : IProcessConfiguration
{
    public TestProcessConfiguration(IConfiguration config)
    {
        // FramesPath is required for some tests
        FramesPath = config["FRAMES_PATH"]!;
        if (string.IsNullOrWhiteSpace(FramesPath))
            throw new ArgumentNullException("FRAMES_PATH");

        // ZipPath optional, fallback to default
        ZipPath = config["ZIP_PATH"] ?? "/home/zip_path";

        // maxRetries from ERROR_MAX_RETRIES or default 3
        var maxRetriesValue = config["ERROR_MAX_RETRIES"];
        if (!int.TryParse(maxRetriesValue, out var parsed))
            parsed = 3;
        maxRetries = parsed;

        AuthServiceUrl = config["AUTH_SERVICE_URL"] ?? string.Empty;
        LoadServiceUrl = config["LOAD_SERVICE_URL"] ?? string.Empty;
        ClientId = config["CLIENT_ID"] ?? string.Empty;
        ClientSecret = config["CLIENT_SECRET"] ?? string.Empty;
        MailHost = config["SMTP_HOST"] ?? string.Empty;
        MailPort = int.TryParse(config["SMTP_PORT"], out var p) ? p : 0;
        MailFrom = config["EMAIL_FROM"] ?? string.Empty;
        MailUsername = config["SMTP_USER"] ?? string.Empty;
        MailPassword = config["SMTP_PASSWORD"] ?? string.Empty;
        SecondsPerFrame = int.TryParse(config["SECONDS_PER_FRAME"], out var s) ? s : 1;
        RabbitMQConnectionString = config["RABBITMQ_CONNECTION"] ?? string.Empty;
        DeliveryServiceUrl = config["DELIVERY_SERVICE_URL"] ?? string.Empty;
    }

    public string FramesPath { get; }

    public string ZipPath { get; }

    public int maxRetries { get; }

    public string AuthServiceUrl { get; }

    public string LoadServiceUrl { get; }

    public string ClientId { get; }

    public string ClientSecret { get; }

    public string MailHost { get; }

    public int MailPort { get; }

    public string MailFrom { get; }

    public string MailUsername { get; }

    public string MailPassword { get; }

    public int SecondsPerFrame { get; }

    public string RabbitMQConnectionString { get; }

    public string DeliveryServiceUrl { get; }

}
