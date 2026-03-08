using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Domain.Entities;
using Movie2Image.Process.Domain.Exceptions;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Application.Mappers;

public static class ProcessingJobMapper
{
	public static ProcessingJob ToDomain(this ProcessMovieDto dto)
	{
		// Validar campos obrigatórios antes de criar value objects
		if (string.IsNullOrWhiteSpace(dto?.Id) ||
			string.IsNullOrWhiteSpace(dto?.UserId) ||
			string.IsNullOrWhiteSpace(dto?.MoviePath))
		{
			throw new ValidationException(["Invalid data: Id, UserId and MoviePath are required"]);
		}

		var id = ProcessingJobId.Create(dto.Id);
		var userId = UserId.Create(dto.UserId);
		var moviePath = MoviePath.Create(dto.MoviePath);
		var job = ProcessingJob.Create(id, userId, moviePath);

		if (!string.IsNullOrWhiteSpace(dto.FramesPath))
		{
			var framesPath = FramesPath.Create(dto.FramesPath);
			job.CompleteFrameExtraction(framesPath);
		}

		if (!string.IsNullOrWhiteSpace(dto.TempZipPath))
		{
			var tempZipPath = ZipPath.Create(dto.TempZipPath);
			job.SetTempZipPath(tempZipPath);
		}

		if (!string.IsNullOrWhiteSpace(dto.ZipPath))
		{
			var zipPath = ZipPath.Create(dto.ZipPath);
			job.CompleteCompression(zipPath);
		}

		// Restore tries if exists
		for (int i = 0; i < dto.Tries; i++)
		{
			try
			{
				job.IncrementTry();
			}
			catch
			{
				// Ignore if max retries exceeded during restore
			}
		}

		if (dto.LastException != null)
		{
			job.Fail(dto.LastException);
		}

		return job;
	}

	public static ProcessMovieDto ToDto(this ProcessingJob job)
	{
		return new ProcessMovieDto
		{
			Id = job.Id.Value,
			UserId = job.UserId.Value,
			MoviePath = job.MoviePath.Value,
			FramesPath = job.FramesPath?.Value,
			TempZipPath = job.TempZipPath?.Value,
			ZipPath = job.ZipPath?.Value,
			Status = job.Status.ToString(),
			Tries = job.Tries,
			LastException = job.LastException
		};
	}

	public static ProcessingJob FromRequest(RequestDto request)
	{
		var id = ProcessingJobId.From(request.CarregamentoId);
		var userId = UserId.From(request.UserId);
		var moviePath = MoviePath.Create(request.CaminhoVideo);

		return ProcessingJob.Create(id, userId, moviePath);
	}
}
