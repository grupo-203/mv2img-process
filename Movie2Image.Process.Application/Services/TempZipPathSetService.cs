using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Application.Services;

public class TempZipPathSetService : ITempZipPathSetService
{

    public void Set(ProcessMovieDto data)
    {
        Validator.Create()
            .Test(!string.IsNullOrWhiteSpace(data?.Id), "Invalid Id")
			.Test(!string.IsNullOrWhiteSpace(data?.FramesPath), "Invalid Frames Path")
			.Validate();

        data!.TempZipPath = Path.Combine(data.FramesPath!, $"frames_{data.Id}.zip");
	}

}
