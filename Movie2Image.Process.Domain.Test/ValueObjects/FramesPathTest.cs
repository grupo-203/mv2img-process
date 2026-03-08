using FluentAssertions;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Domain.Test.ValueObjects;

public class FramesPathTest : TestBase
{
	[Fact]
	public void Create_WithValidValue_ShouldCreateFramesPath()
	{
		var value = Faker.System.FilePath();

		var framesPath = FramesPath.Create(value);

		framesPath.Should().NotBeNull();
		framesPath.Value.Should().Be(value);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WithInvalidValue_ShouldThrowArgumentException(string? invalidValue)
	{
		var act = () => FramesPath.Create(invalidValue);

		act.Should().Throw<ArgumentException>()
			.WithMessage("Frames path cannot be null or empty.*");
	}

	[Fact]
	public void ImplicitConversion_ToString_ShouldReturnValue()
	{
		var value = Faker.System.FilePath();
		var framesPath = FramesPath.Create(value);

		string result = framesPath;

		result.Should().Be(value);
	}
}
