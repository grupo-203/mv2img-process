using FluentAssertions;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Domain.Test.Exceptions;

public class ValidationExceptionTest : TestBase
{
	[Fact]
	public void Constructor_WithErrors_ShouldCreateException()
	{
		var errors = new[] { "Error 1", "Error 2" };

		var exception = new ValidationException(errors);

		exception.Should().NotBeNull();
	}

	[Fact]
	public void Message_WithSingleError_ShouldFormatCorrectly()
	{
		var error = "Single error";
		var errors = new[] { error };

		var exception = new ValidationException(errors);

		exception.Message.Should().Contain("Validation Errors:");
		exception.Message.Should().Contain(error);
	}

	[Fact]
	public void Message_WithMultipleErrors_ShouldFormatWithNewLines()
	{
		var error1 = "First error";
		var error2 = "Second error";
		var errors = new[] { error1, error2 };

		var exception = new ValidationException(errors);

		exception.Message.Should().Contain("Validation Errors:");
		exception.Message.Should().Contain(error1);
		exception.Message.Should().Contain(error2);
	}

	[Fact]
	public void Message_WithEmptyErrors_ShouldOnlyContainPrefix()
	{
		var errors = Array.Empty<string>();

		var exception = new ValidationException(errors);

		exception.Message.Should().Be("Validation Errors: ");
	}
}
