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
		var framesPath = Faker.System.DirectoryPath();
		var data = new ProcessMovieDto
		{
			Id = Faker.Random.Guid().ToString(),
			UserId = Faker.Random.Guid().ToString(),
			MoviePath = Faker.System.FilePath(),
			FramesPath = framesPath,
			Status = "FramesExtracted"
		};
		pathSetter.Setup(x => x.Set(data)).Callback(() =>
			data.TempZipPath = Faker.System.FilePath());

		// Act
		await useCase.Process(data);

		// Assert
		zip.Verify(x => x.Create(framesPath, data.TempZipPath!), Times.Once);
		data.Status.Should().Be("Compressing");
		data.TempZipPath.Should().NotBeNullOrEmpty();
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
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("*Invalid data: Id, UserId and MoviePath are required*");
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
		var data = new ProcessMovieDto
		{
			Id = Faker.Random.Guid().ToString(),
			UserId = Faker.Random.Guid().ToString(),
			MoviePath = Faker.System.FilePath(),
			FramesPath = value
		};

		// Act
		var act = () => useCase.Process(data);

		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("Cannot start compression from current status*");
	}

}
