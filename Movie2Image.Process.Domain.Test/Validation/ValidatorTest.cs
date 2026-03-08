using FluentAssertions;
using Movie2Image.Process.Domain.Exceptions;
using Movie2Image.Process.Domain.Validation;

namespace Movie2Image.Process.Domain.Test.Validation;

public class ValidatorTest : TestBase
{
	[Fact]
	public void Create_ShouldReturnValidatorInstance()
	{
		var validator = Validator.Create();

		validator.Should().NotBeNull();
	}

	[Fact]
	public void Test_WithPassingCondition_ShouldNotAddError()
	{
		var validator = Validator.Create();

		validator.Test(true, "This error should not be added");
		var act = () => validator.Validate();

		act.Should().NotThrow();
	}

	[Fact]
	public void Test_WithFailingCondition_ShouldAddError()
	{
		var validator = Validator.Create();
		var errorMessage = "This is an error";

		validator.Test(false, errorMessage);
		var act = () => validator.Validate();

		act.Should().Throw<ValidationException>()
			.WithMessage($"*{errorMessage}*");
	}

	[Fact]
	public void Test_WithMultipleFailingConditions_ShouldAddAllErrors()
	{
		var validator = Validator.Create();
		var error1 = "First error";
		var error2 = "Second error";

		validator.Test(false, error1);
		validator.Test(false, error2);
		var act = () => validator.Validate();

		act.Should().Throw<ValidationException>()
			.WithMessage($"*{error1}*{error2}*");
	}

	[Fact]
	public void IsNotNull_WithNonNullObject_ShouldNotAddError()
	{
		var validator = Validator.Create();
		var obj = new object();

		validator.IsNotNull(obj);
		var act = () => validator.Validate();

		act.Should().NotThrow();
	}

	[Fact]
	public void IsNotNull_WithNullObject_ShouldAddError()
	{
		var validator = Validator.Create();

		validator.IsNotNull(null!);
		var act = () => validator.Validate();

		act.Should().Throw<ValidationException>()
			.WithMessage("*should not be null*");
	}

	[Fact]
	public void Validate_WithNoErrors_ShouldNotThrow()
	{
		var validator = Validator.Create();

		var act = () => validator.Validate();

		act.Should().NotThrow();
	}

	[Fact]
	public void Test_ShouldReturnValidatorForChaining()
	{
		var validator = Validator.Create();

		var result = validator.Test(true, "Error");

		result.Should().BeSameAs(validator);
	}

	[Fact]
	public void IsNotNull_ShouldReturnValidatorForChaining()
	{
		var validator = Validator.Create();

		var result = validator.IsNotNull(new object());

		result.Should().BeSameAs(validator);
	}
}
