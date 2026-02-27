using Movie2Image.Process.Application.DTO;

namespace Movie2Image.Process.Application.Ports.Input.Controller;

public interface IPublishController
{

	Task Process(ProcessMovieDto data);

}
