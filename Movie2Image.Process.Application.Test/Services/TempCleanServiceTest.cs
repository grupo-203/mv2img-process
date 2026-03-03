using FluentAssertions;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Services;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Application.Test.Services;

public class TempCleanServiceTest : TestBase
{

    [Fact]
    public async Task Clean_ShouldDeleteDirectory_WhenDirectoryExists()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));
        Directory.CreateDirectory(tempPath);
        var testFile = Path.Combine(tempPath, "test.txt");
        File.WriteAllText(testFile, "test content");

        var data = new ProcessMovieDto
        {
            FramesPath = tempPath
        };

        var service = new TempCleanService();

        // Act
        await service.Clean(data);

        // Assert
        Directory.Exists(tempPath).Should().BeFalse();
    }

    [Fact]
    public async Task Clean_ShouldNotThrow_WhenDirectoryDoesNotExist()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));
        var data = new ProcessMovieDto
        {
            FramesPath = nonExistentPath
        };

        var service = new TempCleanService();

        // Act
        var act = async () => await service.Clean(data);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Clean_ShouldThrowValidationException_WhenDataIsNull()
    {
        // Arrange
        var service = new TempCleanService();

        // Act
        var act = async () => await service.Clean(null!);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Invalid Frames Path*");
    }

    [Fact]
    public async Task Clean_ShouldThrowValidationException_WhenFramesPathIsNull()
    {
        // Arrange
        var data = new ProcessMovieDto
        {
            FramesPath = null
        };

        var service = new TempCleanService();

        // Act
        var act = async () => await service.Clean(data);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Invalid Frames Path*");
    }

    [Fact]
    public async Task Clean_ShouldThrowValidationException_WhenFramesPathIsEmpty()
    {
        // Arrange
        var data = new ProcessMovieDto
        {
            FramesPath = string.Empty
        };

        var service = new TempCleanService();

        // Act
        var act = async () => await service.Clean(data);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Invalid Frames Path*");
    }

    [Fact]
    public async Task Clean_ShouldThrowValidationException_WhenFramesPathIsWhitespace()
    {
        // Arrange
        var data = new ProcessMovieDto
        {
            FramesPath = "   "
        };

        var service = new TempCleanService();

        // Act
        var act = async () => await service.Clean(data);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Invalid Frames Path*");
    }

    [Fact]
    public async Task Clean_ShouldDeleteDirectoryRecursively_WhenDirectoryHasSubdirectories()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("n"));
        Directory.CreateDirectory(tempPath);

        var subDir1 = Path.Combine(tempPath, "subdir1");
        var subDir2 = Path.Combine(tempPath, "subdir2");
        Directory.CreateDirectory(subDir1);
        Directory.CreateDirectory(subDir2);

        File.WriteAllText(Path.Combine(tempPath, "file1.txt"), "content1");
        File.WriteAllText(Path.Combine(subDir1, "file2.txt"), "content2");
        File.WriteAllText(Path.Combine(subDir2, "file3.txt"), "content3");

        var data = new ProcessMovieDto
        {
            FramesPath = tempPath
        };

        var service = new TempCleanService();

        // Act
        await service.Clean(data);

        // Assert
        Directory.Exists(tempPath).Should().BeFalse();
        Directory.Exists(subDir1).Should().BeFalse();
        Directory.Exists(subDir2).Should().BeFalse();
    }

}
