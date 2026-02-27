using FluentAssertions;
using Moq;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Output.Zip;
using Movie2Image.Process.Application.UseCases;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Application.Test.UseCases;

public class GenerateZipUseCaseTest : TestBase
{

	[Fact]
	public async Task GenerateZipUseCase_Ok()
	{
		// Arrange
		var pathSetter = new Mock<ITempZipPathSetService>();
		var zip = new Mock<IZipCreate>();
		var useCase = new GenerateZipUseCase(pathSetter.Object, zip.Object);
		var data = new ProcessMovieDto
		{
			FramesPath = Faker.System.DirectoryPath()
		};
		pathSetter.Setup(x => x.Set(data)).Callback(() =>
			data.TempZipPath = Faker.System.FilePath());

		// Act
		await useCase.Process(data);

		// Assert
		zip.Verify(x => x.Create(data.FramesPath, data.TempZipPath!), Times.Once);
		data.Status.Should().Be("Zip Generated");
	}

	[Fact]
	public async Task GenerateZipUseCase_Invalid_Data()
	{
		// Arrange
		var pathSetter = new Mock<ITempZipPathSetService>();
		var zip = new Mock<IZipCreate>();
		var useCase = new GenerateZipUseCase(pathSetter.Object, zip.Object);

		// Act
		var act = () => useCase.Process(null!);

		// Assert
		await act.Should().ThrowAsync<ValidationException>();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(" ")]
	public async Task GenerateZipUseCase_Invalid_FramesPath(string? value)
	{
		// Arrange
		var pathSetter = new Mock<ITempZipPathSetService>();
		var zip = new Mock<IZipCreate>();
		var useCase = new GenerateZipUseCase(pathSetter.Object, zip.Object);
		var data = new ProcessMovieDto { FramesPath = value };

		// Act
		var act = () => useCase.Process(data);

		// Assert
		await act.Should().ThrowAsync<ValidationException>();
	}

}
