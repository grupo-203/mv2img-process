namespace Movie2Image.Process.Application.DTO;

public class RequestDto
{

	public Guid CarregamentoId { get; set; }

	public Guid UserId { get; set; }

	public string? CaminhoVideo { get; set; }

}
