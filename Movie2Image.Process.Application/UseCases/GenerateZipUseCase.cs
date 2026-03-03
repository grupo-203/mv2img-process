using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Mappers;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Output.Zip;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Application.UseCases;

public class GenerateZipUseCase(
	ITempZipPathSetService pathSetter,
	IZipCreate zip) : IGenerateZipUseCase
{

	public async Task Process(ProcessMovieDto data)
	{
		// Converter DTO para entidade de domínio
		var processingJob = data.ToDomain();

		// Iniciar compressão
		processingJob.StartCompression();

		// Configurar caminhos
		pathSetter.Set(data);

		// Criar zip
		await zip.Create(processingJob.FramesPath!.Value, data.TempZipPath!);

		// Completar compressão
		var zipPath = ZipPath.Create(data.TempZipPath);
		processingJob.SetTempZipPath(zipPath);

		// Atualizar DTO com estado da entidade
		data.Status = processingJob.Status.ToString();
		data.TempZipPath = processingJob.TempZipPath?.Value;
	}

}
