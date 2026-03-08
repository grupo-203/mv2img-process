using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.Ports.Input.Queue;
using Movie2Image.Process.Application.Ports.Output.Queue;
using RabbitMQ.Client;

namespace Movie2Image.Process.Queue.RabbitMQ;

public class QueuePublish(
	IConfiguration configuration,
	ILogger<QueuePublish> logger,
	ITransformBody transformBody,
	IChannel channel) : IQueuePublish
{

	private readonly string exchange = configuration["exchange"] ?? "";
	private readonly BasicProperties basic = new BasicProperties();
	private readonly BasicProperties persistent = new BasicProperties
	{
		Persistent = true
	};

	public async Task Publish<T>(string queueName, T message)
	{
		logger.LogInformation($"Publishing message to queue [{queueName}].");

		await InternalPublish(queueName, message, basic);
	}

	public async Task PublishPersistent<T>(string queueName, T message)
	{
		logger.LogInformation($"Publishing persistent message to queue [{queueName}]");

		await InternalPublish(queueName, message, persistent);
	}

	public async Task PublishList<T>(string queueName, params IEnumerable<T> messages)
	{
		logger.LogInformation($"Publishing messages [{messages.Count()}] to queue [{queueName}]");

		var properties = new BasicProperties();

		foreach (var message in messages)
			await InternalPublish(queueName, message, basic);
	}

	public async Task PublishListPersistent<T>(string queueName, params IEnumerable<T> messages)
	{
		logger.LogInformation($"Publishing persistent messages [{messages.Count()}] to queue: {queueName}.");

		var properties = new BasicProperties();

		foreach (var message in messages)
			await InternalPublish(queueName, message, persistent);
	}


	private async Task InternalPublish<T>(string queueName, T message, BasicProperties properties)
	{
		await channel.BasicPublishAsync(
			exchange: exchange,
			routingKey: queueName,
			mandatory: false,
			basicProperties: properties,
			body: transformBody.GetBytes(message)
		);
	}

}
