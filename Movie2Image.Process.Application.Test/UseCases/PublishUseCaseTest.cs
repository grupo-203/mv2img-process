using FluentAssertions;
using Moq;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Output.Storage;
using Movie2Image.Process.Application.UseCases;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Application.Test.UseCases;

public class PublishUseCaseTest : TestBase
{

	[Fact]
	public async Task PublishUseCase_Ok()
	{
		// Arrange
		var pathSetter = new Mock<IZipPathSetService>();
		var storage = new Mock<IStorage>();
		var cleanner = new Mock<ITempCleanService>();
		var useCase = new PublishUseCase(pathSetter.Object, storage.Object, cleanner.Object);
		var data = new ProcessMovieDto
		{
			TempZipPath = GetTempFile()
		};
		var file = Path.GetFileName(data.TempZipPath);
		var zipPath = Path.Combine(Faker.System.DirectoryPath(), file);
		pathSetter.Setup(x => x.Set(data)).Callback(() =>
			data.ZipPath = zipPath);

		// Act
		await useCase.Process(data);

		// Assert
		pathSetter.Verify(x => x.Set(data), Times.Once);
		storage.Verify(x => x.UploadFile(It.IsAny<Stream>(), zipPath), Times.Once);
		cleanner.Verify(x => x.Clean(data), Times.Once);
		data.Status.Should().Be("Published");
	}

	[Fact]
	public async Task PublishUseCase_Invalid_Data()
	{
		// Arrange
		var pathSetter = new Mock<IZipPathSetService>();
		var storage = new Mock<IStorage>();
		var cleanner = new Mock<ITempCleanService>();
		var useCase = new PublishUseCase(pathSetter.Object, storage.Object, cleanner.Object);

		// Act
		var act = () => useCase.Process(null!);

		// Assert
		await act.Should().ThrowAsync<ValidationException>();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(" ")]
	public async Task PublishUseCase_Invalid_TempZipPath(string? value)
	{
		// Arrange
		var pathSetter = new Mock<IZipPathSetService>();
		var storage = new Mock<IStorage>();
		var cleanner = new Mock<ITempCleanService>();
		var useCase = new PublishUseCase(pathSetter.Object, storage.Object, cleanner.Object);
		var data = new ProcessMovieDto
		{
			TempZipPath = value
		};

		// Act
		var act = () => useCase.Process(data);

		// Assert
		await act.Should().ThrowAsync<ValidationException>();
	}

	[Fact]
	public async Task PublishUseCase_Not_Exists_TempZipPath()
	{
		// Arrange
		var pathSetter = new Mock<IZipPathSetService>();
		var storage = new Mock<IStorage>();
		var cleanner = new Mock<ITempCleanService>();
		var useCase = new PublishUseCase(pathSetter.Object, storage.Object, cleanner.Object);
		var data = new ProcessMovieDto
		{
			TempZipPath = Faker.System.FilePath()
		};

		// Act
		var act = () => useCase.Process(data);

		// Assert
		await act.Should().ThrowAsync<ValidationException>();
	}

}
