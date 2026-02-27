using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.Ports.Input.Queue;
using Movie2Image.Process.Application.Ports.Output.Queue;
using Movie2Image.Process.Logging;
using Movie2Image.Process.Queue.Common;
using Movie2Image.Process.Queue.RabbitMQ;
using RabbitMQ.Client;

namespace Movie2Image.Process.Queue;

public static class Configuration
{

	public static IServiceCollection AddQueues(this IServiceCollection services)
	{
		ConsoleLogger.LogInformation("Add [Queues]");

		SetRabbitMQClient(services);
		SetRabbitMQChannel(services);

		services.AddSingleton<IQueueNames, QueueNames>();
		services.AddSingleton<ITransformBody, TransformBody>();
		services.AddSingleton<IQueuePublish, QueuePublish>();
		services.AddSingleton<IQueueList, QueueList>();

		return services;
	}


	private static void SetRabbitMQClient(IServiceCollection services)
	{
		services.AddSingleton(provider =>
		{
			var logger = provider.GetRequiredService<ILogger<QueueList>>();

			logger.LogInformation("Trying to create a RabbitMQ connection");

			var config = provider.GetRequiredService<IConfiguration>();
			var connectionString = config["RABBITMQ_CONNECTION"]
				?? throw new ArgumentNullException("RABBITMQ_CONNECTION");
			var factory = GetFactory(connectionString);
			var connection = factory.CreateConnectionAsync().Result;

			logger.LogInformation("RabbitMQ connection created");

			return connection;
		});
	}

	private static void SetRabbitMQChannel(IServiceCollection services)
	{
		services.AddSingleton(provider =>
		{
			var logger = provider.GetRequiredService<ILogger<QueueList>>();

			logger.LogInformation("Trying to create a RabbitMQ channel");

			var connection = provider.GetRequiredService<IConnection>();
			var channel = connection.CreateChannelAsync().Result;

			logger.LogInformation($"RabbitMQ channel [{channel.ChannelNumber}] created");

			return channel;
		});
	}

	private static IConnectionFactory GetFactory(string connectionString)
	{
		var factory = new ConnectionFactory();
		var info = GetConnectionInfo(connectionString);

		if (info.ContainsKey("hostname"))
			factory.HostName = info["hostname"];

		if (info.ContainsKey("port"))
			factory.Port = int.Parse(info["port"]);

		if (info.ContainsKey("username"))
			factory.UserName = info["username"];

		if (info.ContainsKey("password"))
			factory.Password = info["password"];

		if (info.ContainsKey("connection-timeout"))
			factory.RequestedConnectionTimeout = TimeSpan.Parse(info["connection-timeout"]);

		return factory;
	}

	private static Dictionary<string, string> GetConnectionInfo(string connectionString)
	{
		var info = new Dictionary<string, string>();
		var items = connectionString.Split(";");

		foreach (var item in items)
		{
			info.Add(
				item.Split("=").First().ToLower().Trim(),
				item.Split("=").Last().Trim());
		}

		return info;
	}

}
