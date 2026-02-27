using Movie2Image.Process.Application.DTO;

namespace Movie2Image.Process.Application.Ports.Core.UseCases;

public interface IProcessErrorUseCase
{

	Task<bool> Process(ProcessMovieDto data, Exception ex);

}
