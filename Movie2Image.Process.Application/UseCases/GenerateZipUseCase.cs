using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Output.Zip;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Application.UseCases;

public class GenerateZipUseCase(
	ITempZipPathSetService pathSetter,
	IZipCreate zip) : IGenerateZipUseCase
{

	public async Task Process(ProcessMovieDto data)
	{
		Validator.Create()
			.Test(!string.IsNullOrWhiteSpace(data?.FramesPath), "Invalid Frames Path")
			.Validate();

		pathSetter.Set(data!);

		await zip.Create(data!.FramesPath!, data.TempZipPath!);
		data.Status = "Zip Generated";
	}

}
