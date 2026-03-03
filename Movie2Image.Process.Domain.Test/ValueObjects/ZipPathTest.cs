using FluentAssertions;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Domain.Test.ValueObjects;

public class ZipPathTest : TestBase
{
	[Fact]
	public void Create_WithValidValue_ShouldCreateZipPath()
	{
		var value = Faker.System.FilePath();

		var zipPath = ZipPath.Create(value);

		zipPath.Should().NotBeNull();
		zipPath.Value.Should().Be(value);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WithInvalidValue_ShouldThrowArgumentException(string? invalidValue)
	{
		var act = () => ZipPath.Create(invalidValue);

		act.Should().Throw<ArgumentException>()
			.WithMessage("Zip path cannot be null or empty.*");
	}

	[Fact]
	public void ImplicitConversion_ToString_ShouldReturnValue()
	{
		var value = Faker.System.FilePath();
		var zipPath = ZipPath.Create(value);

		string result = zipPath;

		result.Should().Be(value);
	}
}
