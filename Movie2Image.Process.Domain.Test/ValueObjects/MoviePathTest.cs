using FluentAssertions;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Domain.Test.ValueObjects;

public class MoviePathTest : TestBase
{
	[Fact]
	public void Create_WithValidValue_ShouldCreateMoviePath()
	{
		var value = Faker.System.FilePath();

		var moviePath = MoviePath.Create(value);

		moviePath.Should().NotBeNull();
		moviePath.Value.Should().Be(value);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WithInvalidValue_ShouldThrowArgumentException(string? invalidValue)
	{
		var act = () => MoviePath.Create(invalidValue);

		act.Should().Throw<ArgumentException>()
			.WithMessage("Movie path cannot be null or empty.*");
	}

	[Fact]
	public void ImplicitConversion_ToString_ShouldReturnValue()
	{
		var value = Faker.System.FilePath();
		var moviePath = MoviePath.Create(value);

		string result = moviePath;

		result.Should().Be(value);
	}
}
