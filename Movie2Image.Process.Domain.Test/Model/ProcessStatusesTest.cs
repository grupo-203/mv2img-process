using FluentAssertions;
using Movie2Image.Process.Domain.Model;

namespace Movie2Image.Process.Domain.Test.Model;

public class ProcessStatusesTest : TestBase
{
	[Fact]
	public void Started_ShouldHaveCorrectValue()
	{
		ProcessStatuses.Started.Should().Be("Started");
	}

	[Fact]
	public void Finished_ShouldHaveCorrectValue()
	{
		ProcessStatuses.Finished.Should().Be("Finished");
	}

	[Fact]
	public void Error_ShouldHaveCorrectValue()
	{
		ProcessStatuses.Error.Should().Be("Error");
	}
}
