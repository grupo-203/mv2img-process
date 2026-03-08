using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Input;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Application.Services;

public class ZipPathSetService(
    IProcessConfiguration config) : IZipPathSetService
{

    public void Set(ProcessMovieDto data)
    {
		Validator.Create()
			.Test(!string.IsNullOrWhiteSpace(data?.TempZipPath), "Invalid Temp Zip Path")
			.Validate();

		var file = Path.GetFileName(data!.TempZipPath);

		data!.ZipPath = $"{config.ZipPath}/{file}";
	}

}
