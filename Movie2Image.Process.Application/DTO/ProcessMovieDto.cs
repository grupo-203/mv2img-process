namespace Movie2Image.Process.Application.DTO;

public class ProcessMovieDto
{

	public string? Id { get; set; }

	public string? UserId { get; set; }

	public string? MoviePath { get; set; }

	public string? FramesPath { get; set; }

	public string? TempZipPath { get; set; }

	public string? ZipPath { get; set; }

	public string? Status { get; set; }

	public Exception? LastException { get; set; }

	public int Tries { get; set; }


	public ProcessMovieDto() { }

	public ProcessMovieDto(RequestDto data)
	{
		Id = $"{data.CarregamentoId}";
		UserId = $"{data.UserId}";
		MoviePath = data.CaminhoVideo;
	}

}
