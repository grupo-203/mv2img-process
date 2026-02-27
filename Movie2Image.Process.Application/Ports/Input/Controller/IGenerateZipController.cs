using Movie2Image.Process.Application.DTO;

namespace Movie2Image.Process.Application.Ports.Input.Controller;

public interface IGenerateZipController
{

	Task Process(ProcessMovieDto data);

}
