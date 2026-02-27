using Movie2Image.Process.Application.DTO;

namespace Movie2Image.Process.Application.Ports.Core.Services;

public interface ITempCleanService
{

	Task Clean(ProcessMovieDto data);

}
