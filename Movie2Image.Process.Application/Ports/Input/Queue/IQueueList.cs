namespace Movie2Image.Process.Application.Ports.Input.Queue;

public interface IQueueList
{

	IQueueList Add<TProcessType>(string queueName, int fetchCount, bool durable, bool exclusive, bool autoDelete, bool autoAck = false, bool unzip = false);

	IQueueList Add<TProcessType>(string queueName, bool autoAck = false, bool unzip = false);

	Task Start();

}
