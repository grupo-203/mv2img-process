using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Input.Controller;
using Movie2Image.Process.Application.Ports.Input.Queue;
using Movie2Image.Process.Application.Ports.Output.Queue;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Controller.Controllers;

public class DeadLetterController(
	INotifyUserUseCase notify,
	IQueuePublish publisher,
	IQueueNames queues) : IDeadLetterController
{

	public async Task Process(ProcessMovieDto data)
	{
		Validator.Create()
			.Test(!string.IsNullOrEmpty(data?.Id), "Invalid Id")
			.Validate();

		await publisher.Publish(queues.Error, data);
		await notify.Process(data!);
	}

}
