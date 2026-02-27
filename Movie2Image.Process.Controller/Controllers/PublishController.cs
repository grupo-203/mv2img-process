using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Input.Controller;
using Movie2Image.Process.Application.Ports.Input.Queue;
using Movie2Image.Process.Application.Ports.Output.ExternalServices;
using Movie2Image.Process.Application.Ports.Output.Queue;

namespace Movie2Image.Process.Controller;

public class PublishController(
	ILogger<PublishController> logger,
	IProcessErrorUseCase errorUseCase,
	ILoadService loadService,
	IPublishUseCase useCase,
	IQueuePublish publisher,
	IQueueNames queues) : IPublishController
{

	public async Task Process(ProcessMovieDto data)
	{
		try
		{
			await useCase.Process(data);
			await loadService.FinishProcess(data.Id!, data.ZipPath!);
		}
		catch (Exception ex)
		{
			await ProcessError(data, ex);
		}
	}

	private async Task ProcessError(ProcessMovieDto data, Exception ex)
	{
		logger.LogError(ex, "Cannot publish zip");

		if (await errorUseCase.Process(data, ex))
			await publisher.Publish(queues.Publish, data);
		else
			await publisher.Publish(queues.DeadLetter, data);
	}

}
