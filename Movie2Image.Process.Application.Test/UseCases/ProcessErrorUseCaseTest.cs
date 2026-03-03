using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.UseCases;

namespace Movie2Image.Process.Application.Test.UseCases;

public class ProcessErrorUseCaseTest : TestBase
{

    [Fact]
    public async Task ProcessErrorUseCase_Ok()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["ERROR_MAX_RETRIES"]).Returns("3");
        var useCase = new ProcessErrorUseCase(configMock.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = Faker.System.FilePath(),
            Tries = 0
        };
        var exception = new Exception(Faker.Lorem.Sentence());

        // Act
        var result = await useCase.Process(data, exception);

        // Assert
        result.Should().BeTrue();
        data.Tries.Should().Be(1);
        data.LastException.Should().Be(exception);
    }

    [Fact]
    public async Task ProcessErrorUseCase_Null_Data()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["ERROR_MAX_RETRIES"]).Returns("3");
        var useCase = new ProcessErrorUseCase(configMock.Object);
        var exception = new Exception(Faker.Lorem.Sentence());

        // Act
        var result = await useCase.Process(null!, exception);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessErrorUseCase_Null_Exception()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["ERROR_MAX_RETRIES"]).Returns("3");
        var useCase = new ProcessErrorUseCase(configMock.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = Faker.System.FilePath(),
            Tries = 0
        };

        // Act
        var result = await useCase.Process(data, null!);

        // Assert
        result.Should().BeFalse();
        data.Tries.Should().Be(0);
    }

    [Fact]
    public async Task ProcessErrorUseCase_Max_Retries_Reached()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["ERROR_MAX_RETRIES"]).Returns("3");
        var useCase = new ProcessErrorUseCase(configMock.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = Faker.System.FilePath(),
            Tries = 3
        };
        var exception = new Exception(Faker.Lorem.Sentence());

        // Act
        var result = await useCase.Process(data, exception);

        // Assert
        result.Should().BeFalse();
        data.Tries.Should().Be(3);
        data.LastException.Should().BeNull();
    }

    [Fact]
    public async Task ProcessErrorUseCase_Max_Retries_Exceeded()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["ERROR_MAX_RETRIES"]).Returns("3");
        var useCase = new ProcessErrorUseCase(configMock.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = Faker.System.FilePath(),
            Tries = 5
        };
        var exception = new Exception(Faker.Lorem.Sentence());

        // Act
        var result = await useCase.Process(data, exception);

        // Assert
        result.Should().BeFalse();
        data.Tries.Should().Be(5);
        data.LastException.Should().BeNull();
    }

    [Fact]
    public async Task ProcessErrorUseCase_One_Retry_Left()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["ERROR_MAX_RETRIES"]).Returns("3");
        var useCase = new ProcessErrorUseCase(configMock.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = Faker.System.FilePath(),
            Tries = 2
        };
        var exception = new Exception(Faker.Lorem.Sentence());

        // Act
        var result = await useCase.Process(data, exception);

        // Assert
        result.Should().BeTrue();
        data.Tries.Should().Be(3);
        data.LastException.Should().Be(exception);
        data.Status.Should().Be("Failed");
    }

    [Fact]
    public async Task ProcessErrorUseCase_Default_Max_Retries()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["ERROR_MAX_RETRIES"]).Returns((string?)null);
        var useCase = new ProcessErrorUseCase(configMock.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = Faker.System.FilePath(),
            Tries = 0
        };
        var exception = new Exception(Faker.Lorem.Sentence());

        // Act
        var result = await useCase.Process(data, exception);

        // Assert
        result.Should().BeTrue();
        data.Tries.Should().Be(1);
        data.LastException.Should().Be(exception);
    }

    [Fact]
    public async Task ProcessErrorUseCase_Custom_Max_Retries()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["ERROR_MAX_RETRIES"]).Returns("5");
        var useCase = new ProcessErrorUseCase(configMock.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = Faker.System.FilePath(),
            Tries = 2
        };
        var exception = new Exception(Faker.Lorem.Sentence());

        // Act
        var result = await useCase.Process(data, exception);

        // Assert
        result.Should().BeTrue();
        data.Tries.Should().Be(3);
        data.LastException.Should().Be(exception);
        data.Status.Should().Be("Failed");
    }

    [Fact]
    public async Task ProcessErrorUseCase_Multiple_Tries()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["ERROR_MAX_RETRIES"]).Returns("3");
        var useCase = new ProcessErrorUseCase(configMock.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = Faker.Random.Guid().ToString(),
            MoviePath = Faker.System.FilePath(),
            Tries = 0
        };
        var exception1 = new Exception(Faker.Lorem.Sentence());
        var exception2 = new Exception(Faker.Lorem.Sentence());

        // Act & Assert - First try
        var result1 = await useCase.Process(data, exception1);
        result1.Should().BeTrue();
        data.Tries.Should().Be(1);
        data.LastException.Should().Be(exception1);

        // Act & Assert - Second try
        var result2 = await useCase.Process(data, exception2);
        result2.Should().BeTrue();
        data.Tries.Should().Be(2);
        data.LastException.Should().Be(exception2);

        // Act & Assert - Third try (last allowed)
        var exception3 = new Exception(Faker.Lorem.Sentence());
        var result3 = await useCase.Process(data, exception3);
        result3.Should().BeTrue();
        data.Tries.Should().Be(3);
        data.LastException.Should().Be(exception3);

        // Act & Assert - Fourth try (should be rejected)
        var exception4 = new Exception(Faker.Lorem.Sentence());
        var result4 = await useCase.Process(data, exception4);
        result4.Should().BeFalse();
        data.Tries.Should().Be(3);
        data.LastException.Should().Be(exception3);
    }

}
