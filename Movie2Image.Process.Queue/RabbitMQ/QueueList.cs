using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.Ports.Input.Queue;
using RabbitMQ.Client;

namespace Movie2Image.Process.Queue.RabbitMQ;

public class QueueList(
	IServiceProvider services,
	ILogger<Queue> logger,
	ITransformBody transformBody,
	IConfiguration configuration,
	IChannel channel) : IQueueList
{

	private readonly List<Queue> queues = new List<Queue>();


	public IQueueList Add<TProcessType>(string queueName, int fetchCount, bool durable, bool exclusive, bool autoDelete, bool autoAck = false, bool unzip = false)
	{
		if (!CanInitialize(queueName))
			return this;

		var config = new QueueInfo(queueName, fetchCount, durable, exclusive, autoDelete, typeof(TProcessType), autoAck);
		var queue = new Queue(services, logger, channel, transformBody, config, unzip);

		queues.Add(queue);

		return this;
	}

	public IQueueList Add<TProcessType>(string queueName, bool autoAck = false, bool unzip = false)
	{
		if (!CanInitialize(queueName))
			return this;

		var config = new QueueInfo(queueName, typeof(TProcessType), autoAck);
		var queue = new Queue(services, logger, channel, transformBody, config, unzip);

		queues.Add(queue);

		return this;
	}

	public async Task Start()
	{
		logger.LogInformation("Starting queues.");

		if (queues.IsNullOrEmpty())
			throw new Exception("There is no queue available to start.");

		foreach (var queue in queues)
			await queue.Start();

		new ManualResetEvent(false).WaitOne();
	}


	private bool CanInitialize(string queueName)
	{
		var queueList = configuration["QUEUE_LIST"];

		if (string.IsNullOrEmpty(queueList))
			return true;

		return queueList.Split(' ')
			.Contains(queueName);
	}

}
