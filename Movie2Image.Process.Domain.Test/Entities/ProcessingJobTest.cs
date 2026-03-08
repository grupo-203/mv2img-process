using FluentAssertions;
using Movie2Image.Process.Domain.Entities;
using Movie2Image.Process.Domain.Enums;
using Movie2Image.Process.Domain.Exceptions;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Domain.Test.Entities;

public class ProcessingJobTest : TestBase
{
	[Fact]
	public void Create_WithValidParameters_ShouldCreateProcessingJob()
	{
		var jobId = ProcessingJobId.From(Guid.NewGuid());
		var userId = UserId.From(Guid.NewGuid());
		var moviePath = MoviePath.Create(Faker.System.FilePath());

		var job = ProcessingJob.Create(jobId, userId, moviePath);

		job.Should().NotBeNull();
		job.Id.Should().Be(jobId);
		job.UserId.Should().Be(userId);
		job.MoviePath.Should().Be(moviePath);
		job.Status.Should().Be(ProcessingStatus.Created);
		job.Tries.Should().Be(0);
	}

	[Fact]
	public void StartFrameExtraction_WhenStatusIsCreated_ShouldChangeStatusToExtractingFrames()
	{
		var job = CreateJob();

		job.StartFrameExtraction();

		job.Status.Should().Be(ProcessingStatus.ExtractingFrames);
	}

	[Fact]
	public void StartFrameExtraction_WhenStatusIsNotCreated_ShouldThrowInvalidOperationException()
	{
		var job = CreateJob();
		job.StartFrameExtraction();

		var act = () => job.StartFrameExtraction();

		act.Should().Throw<InvalidOperationException>()
			.WithMessage("Cannot start frame extraction from current status.*");
	}

	[Fact]
	public void CompleteFrameExtraction_WithValidFramesPath_ShouldSetFramesPathAndChangeStatus()
	{
		var job = CreateJob();
		job.StartFrameExtraction();
		var framesPath = FramesPath.Create(Faker.System.FilePath());

		job.CompleteFrameExtraction(framesPath);

		job.FramesPath.Should().Be(framesPath);
		job.Status.Should().Be(ProcessingStatus.FramesExtracted);
	}

	[Fact]
	public void CompleteFrameExtraction_WithNullFramesPath_ShouldThrowArgumentNullException()
	{
		var job = CreateJob();
		job.StartFrameExtraction();

		var act = () => job.CompleteFrameExtraction(null!);

		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void StartCompression_WhenStatusIsFramesExtracted_ShouldChangeStatusToCompressing()
	{
		var job = CreateJob();
		job.StartFrameExtraction();
		job.CompleteFrameExtraction(FramesPath.Create(Faker.System.FilePath()));

		job.StartCompression();

		job.Status.Should().Be(ProcessingStatus.Compressing);
	}

	[Fact]
	public void StartCompression_WhenStatusIsNotFramesExtracted_ShouldThrowInvalidOperationException()
	{
		var job = CreateJob();

		var act = () => job.StartCompression();

		act.Should().Throw<InvalidOperationException>()
			.WithMessage("Cannot start compression from current status.*");
	}

	[Fact]
	public void SetTempZipPath_WithValidZipPath_ShouldSetTempZipPath()
	{
		var job = CreateJob();
		var tempZipPath = ZipPath.Create(Faker.System.FilePath());

		job.SetTempZipPath(tempZipPath);

		job.TempZipPath.Should().Be(tempZipPath);
	}

	[Fact]
	public void SetTempZipPath_WithNullZipPath_ShouldThrowArgumentNullException()
	{
		var job = CreateJob();

		var act = () => job.SetTempZipPath(null!);

		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void CompleteCompression_WithValidZipPath_ShouldSetZipPathAndChangeStatus()
	{
		var job = CreateJob();
		var zipPath = ZipPath.Create(Faker.System.FilePath());

		job.CompleteCompression(zipPath);

		job.ZipPath.Should().Be(zipPath);
		job.Status.Should().Be(ProcessingStatus.Compressed);
	}

	[Fact]
	public void CompleteCompression_WithNullZipPath_ShouldThrowArgumentNullException()
	{
		var job = CreateJob();

		var act = () => job.CompleteCompression(null!);

		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Complete_ShouldChangeStatusToCompleted()
	{
		var job = CreateJob();

		job.Complete();

		job.Status.Should().Be(ProcessingStatus.Completed);
	}

	[Fact]
	public void Fail_WithException_ShouldSetExceptionAndChangeStatusToFailed()
	{
		var job = CreateJob();
		var exception = new Exception("Test error");

		job.Fail(exception);

		job.LastException.Should().Be(exception);
		job.Status.Should().Be(ProcessingStatus.Failed);
	}

	[Fact]
	public void IncrementTry_ShouldIncreaseTriesCount()
	{
		var job = CreateJob();
		var initialTries = job.Tries;

		job.IncrementTry();

		job.Tries.Should().Be(initialTries + 1);
	}

	[Fact]
	public void IncrementTry_WhenExceedingMaxRetries_ShouldThrowMaxRetriesExceededException()
	{
		var job = CreateJob();
		job.IncrementTry();
		job.IncrementTry();
		job.IncrementTry();

		var act = () => job.IncrementTry();

		act.Should().Throw<MaxRetriesExceededException>()
			.WithMessage($"Maximum retry attempts (3) exceeded for processing job*");
	}

	[Fact]
	public void ResetTries_ShouldSetTriesToZero()
	{
		var job = CreateJob();
		job.IncrementTry();
		job.IncrementTry();

		job.ResetTries();

		job.Tries.Should().Be(0);
	}

	[Fact]
	public void CanRetry_WhenTriesLessThanMax_ShouldReturnTrue()
	{
		var job = CreateJob();
		job.IncrementTry();

		var result = job.CanRetry(3);

		result.Should().BeTrue();
	}

	[Fact]
	public void CanRetry_WhenTriesEqualToMax_ShouldReturnFalse()
	{
		var job = CreateJob();
		job.IncrementTry();
		job.IncrementTry();
		job.IncrementTry();

		var result = job.CanRetry(3);

		result.Should().BeFalse();
	}

	private ProcessingJob CreateJob()
	{
		var jobId = ProcessingJobId.From(Guid.NewGuid());
		var userId = UserId.From(Guid.NewGuid());
		var moviePath = MoviePath.Create(Faker.System.FilePath());
		return ProcessingJob.Create(jobId, userId, moviePath);
	}
}
