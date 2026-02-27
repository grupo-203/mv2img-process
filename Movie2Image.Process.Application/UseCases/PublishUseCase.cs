using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Output.Storage;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Application.UseCases;

public class PublishUseCase(
	IZipPathSetService pathSetter,
	IStorage storage,
	ITempCleanService cleanner) : IPublishUseCase
{

	public async Task Process(ProcessMovieDto data)
	{
		Validator.Create()
			.Test(!string.IsNullOrWhiteSpace(data?.TempZipPath), "Invalid Temp Zip Path")
			.Validate();

		Validator.Create()
			.Test(File.Exists(data?.TempZipPath), "Temp Zip File Not Exists")
			.Validate();

		pathSetter.Set(data!);

		using (var file = File.OpenRead(data!.TempZipPath!))
			 await storage.UploadFile(file, data.ZipPath!);

		await cleanner.Clean(data);
		data.Status = "Published";
	}

}
