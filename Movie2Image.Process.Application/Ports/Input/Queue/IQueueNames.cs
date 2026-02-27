namespace Movie2Image.Process.Application.Ports.Input.Queue;

public interface IQueueNames
{

	string ProcessMovie { get; }

	string ExtractFrames { get; }

	string GenerateZip { get; }

	string Publish { get; }

	string Finished { get; }

	string DeadLetter { get; }

	string Error { get; }

}
