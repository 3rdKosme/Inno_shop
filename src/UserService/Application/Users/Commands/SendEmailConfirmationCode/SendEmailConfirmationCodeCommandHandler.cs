using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Microsoft.Extensions.Options;
using MediatR;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Emails;

namespace Inno_Shop.UserService.Application.Users.Commands.SendEmailConfirmationCode;

public class SendEmailConfirmationCodeCommandHandler(IUserRepository userRepository, IEmailService emailService, 
    ITokenRepository<EmailConfirmationToken> emailConfirmationTokenRepository, IOptions<AppSettings> appSettings,
    IOptions<EmailConfirmationTokenSettings> emailConfirmationTokenSettings,
    ITokenGenerator tokenGenerator, ICurrentUserService currentUserService) : IRequestHandler<SendEmailConfirmationCodeCommand, Unit>
{
    public async Task<Unit> Handle(SendEmailConfirmationCodeCommand request, CancellationToken cancellationToken = default)
    {
        var userEmail = currentUserService.Email 
                        ?? throw new UnauthorizedAccessException();

        var user = await userRepository.GetByEmailAsync(userEmail, cancellationToken) 
                   ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        var emailToken = tokenGenerator.GenerateSecureToken();

        var token = new EmailConfirmationToken(user.Id, emailToken, DateTime.UtcNow.AddMinutes(emailConfirmationTokenSettings.Value.ExpireMinutes));

        await emailConfirmationTokenRepository.AddAsync(token, cancellationToken);

        var emailConfirmationLink = $"{appSettings.Value.FrontendUrl}/confirm-email?token={emailToken}";

        await emailService.SendAsync(user.Email, EmailTemplate.EmailConfirmation, new EmailConfirmationModel 
            { ConfirmationLink = emailConfirmationLink }, cancellationToken);

        return Unit.Value;
    }
}