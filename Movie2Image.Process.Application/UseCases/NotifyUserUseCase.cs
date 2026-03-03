using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Mappers;
using Movie2Image.Process.Application.Ports.Core.UseCases;
using Movie2Image.Process.Application.Ports.Output.ExternalServices;
using Movie2Image.Process.Application.Ports.Output.Mail;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Application.UseCases;

public class NotifyUserUseCase(
	IAuthService auth,
	IMailSenderService mail) : INotifyUserUseCase
{

	public async Task Process(ProcessMovieDto data)
	{
		// Converter DTO para entidade de domínio
		var processingJob = data.ToDomain();

		// Obter usuário
		var user = await auth.GetUser(processingJob.UserId);

		Validator.Create()
			.Test(user != null, "User not found")
			.Validate();

		Validator.Create()
			.Test(!string.IsNullOrEmpty(user?.Email), "Invalid User Email")
			.Validate();

		// Enviar email
		await mail.Send(user!.Email!,
			"Erro no processamento do vídeo.",
			$"Não foi possível processar o vídeo {Path.GetFileName(processingJob.MoviePath)}, tente novamente mais tarde.");
	}

}
