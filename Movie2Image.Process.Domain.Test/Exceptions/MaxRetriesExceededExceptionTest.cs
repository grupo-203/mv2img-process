using FluentAssertions;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Domain.Test.Exceptions;

public class MaxRetriesExceededExceptionTest : TestBase
{
	[Fact]
	public void Constructor_WithMessage_ShouldCreateException()
	{
		var message = "Max retries exceeded";

		var exception = new MaxRetriesExceededException(message);

		exception.Should().NotBeNull();
		exception.Message.Should().Be(message);
	}

	[Fact]
	public void Exception_ShouldInheritFromException()
	{
		var exception = new MaxRetriesExceededException("Test");

		exception.Should().BeAssignableTo<Exception>();
	}
}
