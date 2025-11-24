using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Microsoft.Extensions.Options;
using MediatR;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Emails;

namespace Inno_Shop.UserService.Application.Users.Commands.SendPasswordResetCode;

public class ResetPasswordCommandHandler(IUserRepository userRepository, IEmailService emailService, 
    ITokenRepository<PasswordResetToken> passwordResetTokenRepository, IOptions<AppSettings> appSettings,
    IOptions<PasswordResetTokenSettings> passwordResetTokenSettings, 
    ITokenGenerator tokenGenerator) : IRequestHandler<SendPasswordResetCodeCommand, Unit>
{
    public async Task<Unit> Handle(SendPasswordResetCodeCommand request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        var token = tokenGenerator.GenerateSecureToken();

        var resetToken = new PasswordResetToken(user.Id, token, DateTime.UtcNow.AddMinutes(passwordResetTokenSettings.Value.ExpireMinutes));

        await passwordResetTokenRepository.AddAsync(resetToken, cancellationToken);

        var resetLink = $"{appSettings.Value.FrontendUrl}/reset-password?token={token}";

        await emailService.SendAsync(user.Email, EmailTemplate.PasswordReset, new PasswordResetModel { ResetLink = resetLink }, cancellationToken);

        return Unit.Value;
    }
}