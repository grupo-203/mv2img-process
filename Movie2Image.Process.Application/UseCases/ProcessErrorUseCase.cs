using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.UseCases;

namespace Movie2Image.Process.Application.UseCases;

public class ProcessErrorUseCase(
	IConfiguration config) : IProcessErrorUseCase
{

	private readonly int maxRetries = Convert.ToInt32(config["ERROR_MAX_RETRIES"] ?? "3");

	public async Task<bool> Process(ProcessMovieDto data, Exception ex)
	{
		if (data == null || ex == null)
			return false;
		if (data.Tries >= maxRetries)
			return false;

		data.AddTry();
		data.LastException = ex;

		return true;
	}

}