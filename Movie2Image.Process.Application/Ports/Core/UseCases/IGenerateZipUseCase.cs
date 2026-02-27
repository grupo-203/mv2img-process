using Movie2Image.Process.Application.DTO;

namespace Movie2Image.Process.Application.Ports.Core.UseCases;

public interface IGenerateZipUseCase
{

	Task Process(ProcessMovieDto data);

}
