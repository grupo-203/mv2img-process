using Movie2Image.Process.Application.DTO;

namespace Movie2Image.Process.Application.Ports.Core.UseCases;

public interface INotifyUserUseCase
{

	Task Process(ProcessMovieDto data);

}
