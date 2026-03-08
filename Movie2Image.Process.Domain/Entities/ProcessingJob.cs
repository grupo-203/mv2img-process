using Movie2Image.Process.Domain.Enums;
using Movie2Image.Process.Domain.ValueObjects;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Domain.Entities;

public class ProcessingJob
{

	public ProcessingJobId Id { get; private set; }
	public UserId UserId { get; private set; }
	public MoviePath MoviePath { get; private set; }
	public FramesPath? FramesPath { get; private set; }
	public ZipPath? TempZipPath { get; private set; }
	public ZipPath? ZipPath { get; private set; }
	public ProcessingStatus Status { get; private set; }
	public int Tries { get; private set; }
	public Exception? LastException { get; private set; }

	private ProcessingJob() { }

	public static ProcessingJob Create(ProcessingJobId id, UserId userId, MoviePath moviePath)
	{
		return new ProcessingJob
		{
			Id = id,
			UserId = userId,
			MoviePath = moviePath,
			Status = ProcessingStatus.Created,
			Tries = 0
		};
	}

	public void StartFrameExtraction()
	{
		ValidateStatus(ProcessingStatus.Created, "Cannot start frame extraction from current status");
		Status = ProcessingStatus.ExtractingFrames;
	}

	public void CompleteFrameExtraction(FramesPath framesPath)
	{
		FramesPath = framesPath ?? throw new ArgumentNullException(nameof(framesPath));
		Status = ProcessingStatus.FramesExtracted;
	}

	public void StartCompression()
	{
		ValidateStatus(ProcessingStatus.FramesExtracted, "Cannot start compression from current status");
		Status = ProcessingStatus.Compressing;
	}

	public void SetTempZipPath(ZipPath tempZipPath)
	{
		TempZipPath = tempZipPath ?? throw new ArgumentNullException(nameof(tempZipPath));
	}

	public void CompleteCompression(ZipPath zipPath)
	{
		ZipPath = zipPath ?? throw new ArgumentNullException(nameof(zipPath));
		Status = ProcessingStatus.Compressed;
	}

	public void Complete()
	{
		Status = ProcessingStatus.Completed;
	}

	public void Fail(Exception exception)
	{
		LastException = exception;
		Status = ProcessingStatus.Failed;
	}

	public void IncrementTry()
	{
		const int DefaultMaxRetries = 3;
		if (Tries + 1 > DefaultMaxRetries)
		{
			throw new MaxRetriesExceededException($"Maximum retry attempts ({DefaultMaxRetries}) exceeded for processing job {Id}");
		}

		Tries++;
	}

	public void ResetTries()
	{
		Tries = 0;
	}

	public bool CanRetry(int maxRetries) => Tries < maxRetries;

	private void ValidateStatus(ProcessingStatus expectedStatus, string errorMessage)
	{
		if (Status != expectedStatus)
		{
			throw new InvalidOperationException($"{errorMessage}. Current status: {Status}, Expected: {expectedStatus}");
		}
	}
}
