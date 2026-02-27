using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.Ports.Input.Queue;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Diagnostics;
using System.Reflection;

namespace Movie2Image.Process.Queue.RabbitMQ;

public class Queue(
	IServiceProvider services,
	ILogger<Queue> logger,
	IChannel channel,
	ITransformBody transformBody,
	QueueInfo config,
	bool unzip = false)
{

	private AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);


	public async Task Start()
	{
		await Declare();
		await Consume();
	}


	private async Task Declare()
	{
		await channel.QueueDeclareAsync(
			queue: config.QueueName,
			durable: config.Durable,
			exclusive: config.Exclusive,
			autoDelete: config.AutoDelete,
			arguments: null
		);

		logger.LogInformation($"Queue [{config.QueueName}] declared.");
	}

	private async Task Consume()
	{
		// Set prefetch count to limit the number of unacked messages
		await channel.BasicQosAsync(
			prefetchSize: 0,
			prefetchCount: (ushort)config.FetchCount,
			global: false);

		consumer.ReceivedAsync += Consumer_ReceivedAsync;

		var consumerName = await channel.BasicConsumeAsync(
			queue: config.QueueName,
			autoAck: config.AutoAck,
			consumer: consumer
		);

		logger.LogInformation($"Consumer created [{config.QueueName}] with prefetch count [{config.FetchCount}]");
		logger.LogInformation($"Queue [{config.QueueName}] waiting for messages.");
	}


	private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs e)
	{
		logger.LogInformation($"Queue [{config.QueueName}] received a message.");

		var processType = config?.ProcessType
			?? throw new ArgumentNullException("ProcessType");
		var process = services.GetRequiredService(processType);
		var method = process.GetType().GetMethod("Process")
			?? throw new ArgumentNullException("Method");
		var parameters = method.GetParameters();

		if (parameters.Length == 0)
			await Execute(config.QueueName, e.DeliveryTag, method, process);
		else
			await Execute(config.QueueName, e.DeliveryTag, method, process, parameters[0].ParameterType, e.Body);
	}

	private async Task Execute(string queueName, ulong tag, MethodInfo method, object process)
	{
		try
		{
			var result = method.Invoke(process, null) as Task;

			if (result != null)
				await result;

			await Ack(tag);
		}
		catch
		{
			await Nack(tag);

			throw;
		}
	}

	private async Task Execute(string queueName, ulong tag, MethodInfo method, object process, Type bodyType, ReadOnlyMemory<byte> body)
	{
		try
		{
			var data = transformBody.GetObject(body, bodyType, unzip);
			var result = method.Invoke(process, [data]) as Task;

			if (result != null)
				await result;

			await Ack(tag);
		}
		catch
		{
			await Nack(tag);

			throw;
		}
	}

	private async Task Ack(ulong tag)
	{
		try
		{
			await channel.BasicAckAsync(
				deliveryTag: tag,
				multiple: false);

			logger.LogInformation($"Queue [{config.QueueName}] message [ACK].");
		}
		catch (AlreadyClosedException ex)
		{
			Debug.WriteLine(ex);
		}
	}

	private async Task Nack(ulong tag)
	{
		try
		{
			await channel.BasicNackAsync(
				deliveryTag: tag,
				multiple: false,
				requeue: true);

			logger.LogInformation($"Queue [{config.QueueName}] message [NACK].");
		}
		catch (AlreadyClosedException ex)
		{
			Debug.WriteLine(ex);
		}
	}
}