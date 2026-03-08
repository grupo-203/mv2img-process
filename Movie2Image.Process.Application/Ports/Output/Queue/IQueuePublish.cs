namespace Movie2Image.Process.Application.Ports.Output.Queue;

public interface IQueuePublish
{

	Task Publish<T>(string queueName, T message);

	Task PublishPersistent<T>(string queueName, T message);

	Task PublishList<T>(string queueName, params IEnumerable<T> messages);

	Task PublishListPersistent<T>(string queueName, params IEnumerable<T> messages);

}
