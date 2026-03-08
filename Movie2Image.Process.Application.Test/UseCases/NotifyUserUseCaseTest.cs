using FluentAssertions;
using Moq;
using Movie2Image.Process.Application.DTO;
using Movie2Image.Process.Application.Ports.Output.ExternalServices;
using Movie2Image.Process.Application.Ports.Output.Mail;
using Movie2Image.Process.Application.UseCases;
using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Application.Test.UseCases;

public class NotifyUserUseCaseTest : TestBase
{

    [Fact]
    public async Task NotifyUserUseCase_Ok()
    {
        // Arrange
        var authService = new Mock<IAuthService>();
        var mailService = new Mock<IMailSenderService>();
        var useCase = new NotifyUserUseCase(authService.Object, mailService.Object);
        var userId = Faker.Random.Guid().ToString();
        var userEmail = Faker.Internet.Email();
        var moviePath = Faker.System.FilePath();
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = userId,
            MoviePath = moviePath
        };
        var user = new UserDto
        {
            Id = userId,
            Email = userEmail
        };
        authService.Setup(x => x.GetUser(userId)).ReturnsAsync(user);

        // Act
        await useCase.Process(data);

        // Assert
        authService.Verify(x => x.GetUser(userId), Times.Once);
        mailService.Verify(x => x.Send(
            userEmail,
            "Erro no processamento do vídeo.",
            $"Não foi possível processar o vídeo {Path.GetFileName(moviePath)}, tente novamente mais tarde."),
            Times.Once);
    }

    [Fact]
    public async Task NotifyUserUseCase_Null_Data()
    {
        // Arrange
        var authService = new Mock<IAuthService>();
        var mailService = new Mock<IMailSenderService>();
        var useCase = new NotifyUserUseCase(authService.Object, mailService.Object);

        // Act
        var act = () => useCase.Process(null!);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Invalid data: Id, UserId and MoviePath are required*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task NotifyUserUseCase_Invalid_UserId(string? userId)
    {
        // Arrange
        var authService = new Mock<IAuthService>();
        var mailService = new Mock<IMailSenderService>();
        var useCase = new NotifyUserUseCase(authService.Object, mailService.Object);
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = userId,
            MoviePath = Faker.System.FilePath()
        };

        // Act
        var act = () => useCase.Process(data);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        authService.Verify(x => x.GetUser(It.IsAny<string>()), Times.Never);
        mailService.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task NotifyUserUseCase_User_Not_Found()
    {
        // Arrange
        var authService = new Mock<IAuthService>();
        var mailService = new Mock<IMailSenderService>();
        var useCase = new NotifyUserUseCase(authService.Object, mailService.Object);
        var userId = Faker.Random.Guid().ToString();
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = userId,
            MoviePath = Faker.System.FilePath()
        };
        authService.Setup(x => x.GetUser(userId)).ReturnsAsync((UserDto?)null);

        // Act
        var act = () => useCase.Process(data);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*User not found*");
        authService.Verify(x => x.GetUser(userId), Times.Once);
        mailService.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task NotifyUserUseCase_Invalid_User_Email(string? email)
    {
        // Arrange
        var authService = new Mock<IAuthService>();
        var mailService = new Mock<IMailSenderService>();
        var useCase = new NotifyUserUseCase(authService.Object, mailService.Object);
        var userId = Faker.Random.Guid().ToString();
        var data = new ProcessMovieDto
        {
            Id = Faker.Random.Guid().ToString(),
            UserId = userId,
            MoviePath = Faker.System.FilePath()
        };
        var user = new UserDto
        {
            Id = userId,
            Email = email
        };
        authService.Setup(x => x.GetUser(userId)).ReturnsAsync(user);

        // Act
        var act = () => useCase.Process(data);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Invalid User Email*");
        authService.Verify(x => x.GetUser(userId), Times.Once);
        mailService.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

}
