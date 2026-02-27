using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Input.Controller;
using Movie2Image.Process.Application.Ports.Output.ExternalServices;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Controller.Controllers;

public class DeadLetterController(
	ILoadService loadService) : IDeadLetterController
{

	public async Task Process(ProcessMovieDto data)
	{
		Validator.Create()
			.Test(!string.IsNullOrEmpty(data?.Id), "Invalid Id")
			.Validate();

		await loadService.ErrorProcess(data!.Id!);
	}

}
