using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Mappers;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Output.Storage;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Application.UseCases;

public class PublishUseCase(
	IZipPathSetService pathSetter,
	IStorage storage,
	ITempCleanService cleanner) : IPublishUseCase
{

	public async Task Process(ProcessMovieDto data)
	{
		// Converter DTO para entidade de domínio
		var processingJob = data.ToDomain();

		// Validar que o arquivo temporário existe
		if (string.IsNullOrWhiteSpace(data.TempZipPath) || !File.Exists(data.TempZipPath))
		{
			throw new InvalidOperationException("Temp zip file does not exist");
		}

		// Configurar caminho do zip final
		pathSetter.Set(data);

		// Upload do arquivo
		using (var file = File.OpenRead(data.TempZipPath))
			await storage.UploadFile(file, data.ZipPath!);

		// Completar compressão com o caminho final
		var zipPath = ZipPath.Create(data.ZipPath);
		processingJob.CompleteCompression(zipPath);

		// Limpar arquivos temporários
		await cleanner.Clean(data);

		// Completar o processamento
		processingJob.Complete();

		// Atualizar DTO com estado da entidade
		data.Status = processingJob.Status.ToString();
		data.ZipPath = processingJob.ZipPath?.Value;
	}

}
