using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Application.Services;

public class TempCleanService : ITempCleanService
{

    public async Task Clean(ProcessMovieDto data)
    {
        Validator.Create()
            .Test(!string.IsNullOrWhiteSpace(data?.FramesPath), "Invalid Frames Path")
            .Validate();

        if (!Directory.Exists(data!.FramesPath))
            return;

        Directory.Delete(data.FramesPath, true);
	}

}
