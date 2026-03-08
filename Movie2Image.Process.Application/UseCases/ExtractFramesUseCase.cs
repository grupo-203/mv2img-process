using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Mappers;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Output.Media;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Application.UseCases;

public class ExtractFramesUseCase(
	IFramesPathSetService pathSetter,
	IMovieIntoImagesTransform transformer) : IExtractFramesUseCase
{

	public async Task Process(ProcessMovieDto data)
	{
		// Converter DTO para entidade de domínio
		var processingJob = data.ToDomain();

		// Configurar caminhos
		pathSetter.Set(data);

		// Executar transformação
		await transformer.Transform(processingJob.MoviePath, data.FramesPath!);

		// Completar extração de frames
		var framesPath = FramesPath.Create(data.FramesPath);
		processingJob.CompleteFrameExtraction(framesPath);

		// Atualizar DTO com estado da entidade
		data.Status = processingJob.Status.ToString();
		data.FramesPath = processingJob.FramesPath?.Value;
	}

}
