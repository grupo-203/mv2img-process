using FluentAssertions;
using Moq;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Core.Services;
using Movie2Image.Process.Application.Ports.Output.Media;
using Movie2Image.Process.Application.UseCases;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Application.Test.UseCases;

public class ExtractFramesUseCaseTest : TestBase
{

    [Fact]
    public async Task ExtractFramesUseCase_Ok()
    {
        // Arrange
        var pathSetter = new Mock<IFramesPathSetService>();
        var transformer = new Mock<IMovieIntoImagesTransform>();
        var useCase = new ExtractFramesUseCase(pathSetter.Object, transformer.Object);
        var moviePath = Faker.System.FilePath();
        var framesPath = Faker.System.DirectoryPath();
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = moviePath
        };
        pathSetter.Setup(x => x.Set(data)).Callback(() =>
            data.FramesPath = framesPath);

        // Act
        await useCase.Process(data);

        // Assert
        pathSetter.Verify(x => x.Set(data), Times.Once);
        transformer.Verify(x => x.Transform(moviePath, framesPath), Times.Once);
        data.Status.Should().Be("FramesExtracted");
        data.FramesPath.Should().Be(framesPath);
    }

    [Fact]
    public async Task ExtractFramesUseCase_Invalid_Data()
    {
        // Arrange
        var pathSetter = new Mock<IFramesPathSetService>();
        var transformer = new Mock<IMovieIntoImagesTransform>();
        var useCase = new ExtractFramesUseCase(pathSetter.Object, transformer.Object);

        // Act
        var act = () => useCase.Process(null!);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Invalid data: Id, UserId and MoviePath are required*");
        pathSetter.Verify(x => x.Set(It.IsAny<ProcessMovieDto>()), Times.Never);
        transformer.Verify(x => x.Transform(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ExtractFramesUseCase_Invalid_MoviePath(string? moviePath)
    {
        // Arrange
        var pathSetter = new Mock<IFramesPathSetService>();
        var transformer = new Mock<IMovieIntoImagesTransform>();
        var useCase = new ExtractFramesUseCase(pathSetter.Object, transformer.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = moviePath
        };

        // Act
        var act = () => useCase.Process(data);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        pathSetter.Verify(x => x.Set(It.IsAny<ProcessMovieDto>()), Times.Never);
        transformer.Verify(x => x.Transform(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ExtractFramesUseCase_PathSetter_Sets_FramesPath()
    {
        // Arrange
        var pathSetter = new Mock<IFramesPathSetService>();
        var transformer = new Mock<IMovieIntoImagesTransform>();
        var useCase = new ExtractFramesUseCase(pathSetter.Object, transformer.Object);
        var moviePath = Faker.System.FilePath();
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = moviePath
        };
        pathSetter.Setup(x => x.Set(data)).Callback(() =>
            data.FramesPath = Faker.System.DirectoryPath());

        // Act
        await useCase.Process(data);

        // Assert
        data.FramesPath.Should().NotBeNullOrEmpty();
        pathSetter.Verify(x => x.Set(data), Times.Once);
        transformer.Verify(x => x.Transform(moviePath, data.FramesPath), Times.Once);
        data.Status.Should().Be("FramesExtracted");
    }

}
