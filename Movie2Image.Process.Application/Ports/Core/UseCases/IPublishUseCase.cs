using Movie2Image.Process.Application.DTO;

namespace Movie2Image.Process.Application.Ports.Core.UseCases;

public interface IPublishUseCase
{

	Task Process(ProcessMovieDto data);

}
