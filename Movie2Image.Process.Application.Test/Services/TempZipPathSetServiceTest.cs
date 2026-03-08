using FluentAssertions;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Services;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Application.Test.Services;

public class TempZipPathSetServiceTest : TestBase
{

	[Fact]
	public void ZipPathSetServiceTest_Ok()
	{
		// Arrange
		var data  = new ProcessMovieDto
		{
			Id = "123",
			FramesPath = @"C:\temp\frames"
		};
		var service = new TempZipPathSetService();

		// Act
		service.Set(data);

		// Assert
		data.Should().NotBeNull();
		data.TempZipPath.Should().Be(Path.Combine(@"C:\temp\frames", "frames_123.zip"));
	}

	[Theory]
	[InlineData(null)]
	[InlineData(" ")]
	[InlineData("")]
	public void ZipPathSetServiceTest_Invalid_Id(string? value)
	{
		// Arrange
		var data = new ProcessMovieDto
		{
			Id = value,
			FramesPath = @"C:\temp\frames"
		};
		var service = new TempZipPathSetService();

		// Act
		var act = () => service.Set(data);

		// Assert
		act.Should().Throw<ValidationException>();
	}

	[Theory]
	[InlineData(null)]
	[InlineData(" ")]
	[InlineData("")]
	public void ZipPathSetServiceTest_Invalid_FramesPath(string? value)
	{
		// Arrange
		var data = new ProcessMovieDto
		{
			Id = "123",
			FramesPath = value
		};
		var service = new TempZipPathSetService();

		// Act
		var act = () => service.Set(data);

		// Assert
		act.Should().Throw<ValidationException>();
	}


}
