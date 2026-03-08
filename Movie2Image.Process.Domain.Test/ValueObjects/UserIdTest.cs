using FluentAssertions;
using Movie2Image.Process.Domain.ValueObjects;

namespace Movie2Image.Process.Domain.Test.ValueObjects;

public class UserIdTest : TestBase
{
	[Fact]
	public void Create_WithValidValue_ShouldCreateUserId()
	{
		var value = Faker.Random.AlphaNumeric(10);

		var userId = UserId.Create(value);

		userId.Should().NotBeNull();
		userId.Value.Should().Be(value);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Create_WithInvalidValue_ShouldThrowArgumentException(string? invalidValue)
	{
		var act = () => UserId.Create(invalidValue);

		act.Should().Throw<ArgumentException>()
			.WithMessage("User ID cannot be null or empty.*");
	}

	[Fact]
	public void From_WithGuid_ShouldCreateUserId()
	{
		var guid = Guid.NewGuid();

		var userId = UserId.From(guid);

		userId.Should().NotBeNull();
		userId.Value.Should().Be(guid.ToString());
	}

	[Fact]
	public void ImplicitConversion_ToString_ShouldReturnValue()
	{
		var value = Faker.Random.AlphaNumeric(10);
		var userId = UserId.Create(value);

		string result = userId;

		result.Should().Be(value);
	}
}
