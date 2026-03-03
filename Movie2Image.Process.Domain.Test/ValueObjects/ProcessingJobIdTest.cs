using FluentAssertions;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Domain.Test.ValueObjects;

public class ProcessingJobIdTest : TestBase
{
	[Fact]
	public void Create_WithValidValue_ShouldCreateProcessingJobId()
	{
		var value = Faker.Random.AlphaNumeric(10);

		var jobId = ProcessingJobId.Create(value);

		jobId.Should().NotBeNull();
		jobId.Value.Should().Be(value);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WithInvalidValue_ShouldThrowArgumentException(string? invalidValue)
	{
		var act = () => ProcessingJobId.Create(invalidValue);

		act.Should().Throw<ArgumentException>()
			.WithMessage("Processing job ID cannot be null or empty.*");
	}

	[Fact]
	public void From_WithGuid_ShouldCreateProcessingJobId()
	{
		var guid = Guid.NewGuid();

		var jobId = ProcessingJobId.From(guid);

		jobId.Should().NotBeNull();
		jobId.Value.Should().Be(guid.ToString());
	}

	[Fact]
	public void ImplicitConversion_ToString_ShouldReturnValue()
	{
		var value = Faker.Random.AlphaNumeric(10);
		var jobId = ProcessingJobId.Create(value);

		string result = jobId;

		result.Should().Be(value);
	}
}
