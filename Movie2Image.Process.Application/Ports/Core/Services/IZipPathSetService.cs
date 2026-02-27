using Movie2Image.Process.Application.DTO;

namespace Movie2Image.Process.Application.Ports.Core.Services;

public interface IZipPathSetService
{

	void Set(ProcessMovieDto data);

}
