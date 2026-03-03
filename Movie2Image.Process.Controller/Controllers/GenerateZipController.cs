using Microsoft.Extensions.Logging;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Mappers;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Input.Controller;
using Movie2Image.Process.Application.Ports.Input.Queue;
using Movie2Image.Process.Application.Ports.Output.Queue;

namespace Movie2Image.Process.Controller;

public class GenerateZipController(
	ILogger<GenerateZipController> logger,
	IProcessErrorUseCase errorUseCase,
	IGenerateZipUseCase useCase,
	IQueuePublish publisher,
	IQueueNames queues) : IGenerateZipController
{

	public async Task Process(ProcessMovieDto data)
	{
		try
		{
			await useCase.Process(data);

			// Resetar tentativas através da entidade de domínio
			var processingJob = data.ToDomain();
			processingJob.ResetTries();
			data.Tries = processingJob.Tries;

			await publisher.Publish(queues.Publish, data);
		}
		catch (Exception ex)
		{
			await ProcessError(data, ex);
		}
	}


	private async Task ProcessError(ProcessMovieDto data, Exception ex)
	{
		logger.LogError(ex, "Cannot generate zip");

		if (await errorUseCase.Process(data, ex))
			await publisher.Publish(queues.Publish, data);
		else
			await publisher.Publish(queues.DeadLetter, data);
	}

}
