using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;

namespace Movie2Image.Process.Application.Services;

public class FramesPathSetService(
	IConfiguration config) : IFramesPathSetService
{

	private readonly string basePath = config["FRAMES_PATH"] 
		?? throw new ArgumentNullException("FRAMES_PATH");

	public void Set(ProcessMovieDto data)
	{
		data.FramesPath = Path.Combine(basePath, Guid.NewGuid().ToString("n"));
	}

}
