using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Input;

namespace Movie2Image.Process.Application.Services;

public class FramesPathSetService(
	IProcessConfiguration config) : IFramesPathSetService
{

	public void Set(ProcessMovieDto data)
	{
		data.FramesPath = Path.Combine(config.FramesPath, Guid.NewGuid().ToString("n"));
	}

}
