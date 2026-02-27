using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Output.Media;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Application.UseCases;

public class ExtractFramesUseCase(
	IFramesPathSetService pathSetter,
	IMovieIntoImagesTransform transformer) : IExtractFramesUseCase
{

	public async Task Process(ProcessMovieDto data)
	{
		var moviePath = data?.MoviePath;

		Validator.Create()
			.Test(!string.IsNullOrEmpty(moviePath), "Invalid MoviePath")
			.Validate();

		pathSetter.Set(data!);
		await transformer.Transform(moviePath!, data!.FramesPath!);
		data.Status = "Frames Extracted";
	}

}
