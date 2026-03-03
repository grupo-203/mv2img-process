using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Services;

namespace Movie2Image.Process.Application.Test.Services;

public class FramesPathSetServiceTest : TestBase
{

    [Fact]
    public void FramesPathSetServiceTest_Ok()
    {
        // Arrange
        var basePath = @"C:\temp\frames";
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "FRAMES_PATH", basePath }
            })
            .Build();
        var data = new ProcessMovieDto();
        var service = new FramesPathSetService(config);

        // Act
        service.Set(data);

        // Assert
        data.Should().NotBeNull();
        data.FramesPath.Should().NotBeNullOrWhiteSpace();
        data.FramesPath.Should().StartWith(basePath);
        Path.GetDirectoryName(data.FramesPath).Should().Be(basePath);
        var guidPart = Path.GetFileName(data.FramesPath);
        guidPart.Should().NotBeNullOrWhiteSpace();
        guidPart.Should().HaveLength(32); // GUID without hyphens (format "n")
    }

    [Fact]
    public void FramesPathSetServiceTest_GeneratesUniqueGuids()
    {
        // Arrange
        var basePath = @"C:\temp\frames";
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "FRAMES_PATH", basePath }
            })
            .Build();
        var data1 = new ProcessMovieDto();
        var data2 = new ProcessMovieDto();
        var service = new FramesPathSetService(config);

        // Act
        service.Set(data1);
        service.Set(data2);

        // Assert
        data1.FramesPath.Should().NotBe(data2.FramesPath);
    }

    [Fact]
    public void FramesPathSetServiceTest_ThrowsException_WhenConfigMissing()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act
        var act = () => new FramesPathSetService(config);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("FRAMES_PATH");
    }

    [Fact]
    public void FramesPathSetServiceTest_ThrowsException_WhenConfigNull()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "FRAMES_PATH", null }
            })
            .Build();

        // Act
        var act = () => new FramesPathSetService(config);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("FRAMES_PATH");
    }

}
