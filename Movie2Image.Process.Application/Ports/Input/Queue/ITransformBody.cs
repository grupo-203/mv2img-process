namespace Movie2Image.Process.Application.Ports.Input.Queue;

public interface ITransformBody
{

	ReadOnlyMemory<byte> GetBytes(object? message);

	object? GetObject(ReadOnlyMemory<byte> message, Type type, bool unzip = false);

}
