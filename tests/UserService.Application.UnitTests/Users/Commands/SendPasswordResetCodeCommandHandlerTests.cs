using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Users.Commands.SendPasswordResetCode;
using Inno_Shop.UserService.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Users.Commands;

public class SendPasswordResetCodeCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<ITokenRepository<PasswordResetToken>> _tokenRepo = new();
    private readonly Mock<ITokenGenerator> _tokenGenerator = new();

    private readonly IOptions<AppSettings> _appSettings =
        Options.Create(new AppSettings { FrontendUrl = "https://frontend.test" });

    private readonly IOptions<PasswordResetTokenSettings> _tokenSettings =
        Options.Create(new PasswordResetTokenSettings { ExpireMinutes = 30 });

    private SendPasswordResetCodeCommandHandler CreateHandler()
    {
        return new SendPasswordResetCodeCommandHandler(_userRepo.Object, _emailService.Object, _tokenRepo.Object,
            _appSettings, _tokenSettings,
            _tokenGenerator.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        _userRepo.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new SendPasswordResetCodeCommand("test@example.com")));
    }

    [Fact]
    public async Task Handle_ShouldGenerateToken_SaveToken_AndSendEmail()
    {
        var email = "user@example.com";
        var user = User.Create("User", email, "pwd");
        typeof(User).GetProperty("Id")!
            .SetValue(user, 1);

        _userRepo.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _tokenGenerator.Setup(t => t.GenerateSecureToken()).Returns("secure-token");

        var handler = CreateHandler();

        await handler.Handle(new SendPasswordResetCodeCommand(email));

        _tokenRepo.Verify(r =>
            r.AddAsync(It.Is<PasswordResetToken>(t =>
                t.UserId == 1 &&
                t.Token == "secure-token"), It.IsAny<CancellationToken>()), Times.Once);

        _emailService.Verify(e =>
            e.SendAsync(email,
                EmailTemplate.PasswordReset,
                It.Is<PasswordResetModel>(m =>
                    m.ResetLink == "https://frontend.test/reset-password?token=secure-token"),
                It.IsAny<CancellationToken>()), Times.Once);
    }
}