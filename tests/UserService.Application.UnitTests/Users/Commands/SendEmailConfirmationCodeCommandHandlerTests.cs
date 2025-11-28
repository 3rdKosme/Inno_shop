using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Users.Commands.SendEmailConfirmationCode;
using Inno_Shop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class SendEmailConfirmationCodeCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<ITokenRepository<EmailConfirmationToken>> _tokenRepo = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();

    private readonly IOptions<AppSettings> _appSettings =
        Options.Create(new AppSettings { FrontendUrl = "https://frontend.test" });

    private readonly IOptions<EmailConfirmationTokenSettings> _tokenSettings =
        Options.Create(new EmailConfirmationTokenSettings { ExpireMinutes = 30 });

    private SendEmailConfirmationCodeCommandHandler CreateHandler()
    {
        return new SendEmailConfirmationCodeCommandHandler(
            _userRepo.Object,
            _emailService.Object,
            _tokenRepo.Object,
            _appSettings,
            _tokenSettings,
            _tokenGenerator.Object,
            _currentUser.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenEmailIsNull()
    {
        _currentUser.Setup(x => x.Email).Returns((string?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(()
            => handler.Handle(new SendEmailConfirmationCodeCommand()));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        _currentUser.Setup(x => x.Email).Returns("test@example.com");

        _userRepo.Setup(x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(()
            => handler.Handle(new SendEmailConfirmationCodeCommand()));
    }

    [Fact]
    public async Task Handle_ShouldGenerateToken_SaveToken_AndSendEmail()
    {
        var email = "user@example.com";

        _currentUser.Setup(x => x.Email).Returns(email);

        var user = User.Create("User", email, "pwd");
        typeof(User)
            .GetProperty("Id")!
            .SetValue(user, 1);

        _userRepo.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _tokenGenerator.Setup(x => x.GenerateSecureToken()).Returns("secure-token");

        var handler = CreateHandler();

        await handler.Handle(new SendEmailConfirmationCodeCommand());

        _tokenRepo.Verify(r =>
                r.AddAsync(
                    It.Is<EmailConfirmationToken>(t =>
                        t.UserId == 1 &&
                        t.Token == "secure-token"),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        _emailService.Verify(e =>
                e.SendAsync(
                    email,
                    EmailTemplate.EmailConfirmation,
                    It.Is<EmailConfirmationModel>(m =>
                        m.ConfirmationLink ==
                        "https://frontend.test/confirm-email?token=secure-token"),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}