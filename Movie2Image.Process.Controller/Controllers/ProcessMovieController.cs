using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Input.Controller;
using Movie2Image.Process.Application.Ports.Input.Queue;
using Movie2Image.Process.Application.Ports.Output.Queue;

namespace Movie2Image.Process.Controller;

public class ProcessMovieController(
    IQueueNames queues,
    IQueuePublish publisher) : IProcessMovieController
{

    public async Task Process(RequestDto data)
    {
        var processData = new ProcessMovieDto(data);

        await publisher.Publish(queues.ExtractFrames, processData);
	}

}
