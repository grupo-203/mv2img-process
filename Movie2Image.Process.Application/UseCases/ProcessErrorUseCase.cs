using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Mappers;
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

		// Converter DTO para entidade de domínio
		var processingJob = data.ToDomain();

		// Verificar se pode tentar novamente
		if (!processingJob.CanRetry())
			return false;

		// Incrementar tentativa e marcar falha
		try
		{
			processingJob.IncrementTry();
			processingJob.Fail(ex);

			// Atualizar DTO com estado da entidade
			data.Tries = processingJob.Tries;
			data.LastException = processingJob.LastException;
			data.Status = processingJob.Status.ToString();

			return true;
		}
		catch
		{
			return false;
		}
	}

}