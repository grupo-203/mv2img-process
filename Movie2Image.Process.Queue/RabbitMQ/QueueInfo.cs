namespace Movie2Image.Process.Queue.RabbitMQ;

public class QueueInfo
{

	public string QueueName { get; private set; }

	public bool AutoAck { get; private set; }

	public int FetchCount { get; private set; }

	public bool Durable { get; private set; }

	public bool Exclusive { get; private set; }

	public bool AutoDelete { get; private set; }

	public Type ProcessType { get; private set; }



	private QueueInfo()
		: this(string.Empty, 1, true, false, false, null!, false) { }

	public QueueInfo(string queueName, Type processType, bool autoAck = false)
		: this(queueName, 1, true, false, false, processType, autoAck) { }

	public QueueInfo(
		string queueName, int fetchCount, bool durable, bool exclusive,
		bool autoDelete, Type processType, bool autoAck = false)
	{
		FetchCount = fetchCount;
		AutoDelete = autoDelete;
		QueueName = queueName;
		Exclusive = exclusive;
		AutoAck = autoAck;
		Durable = durable;
		ProcessType = processType;
	}


}
