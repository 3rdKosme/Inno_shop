using Inno_Shop.Shared.Application.Exceptions;
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
    IEmailConfirmationTokenRepository emailConfirmationTokenRepository, IOptions<AppSettings> appSettings,
    IOptions<EmailConfirmationTokenSettings> emailConfirmationTokenSettings,
    ITokenGenerator tokenGenerator, ICurrentUserService currentUserService) : IRequestHandler<SendEmailConfirmationCodeCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailConfirmationTokenRepository _emailConfirmationTokenRepository = emailConfirmationTokenRepository;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly EmailConfirmationTokenSettings _emailConfirmationTokenSettings = emailConfirmationTokenSettings.Value;
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<Unit> Handle(SendEmailConfirmationCodeCommand request, CancellationToken cancellationToken = default)
    {
        var userEmail = _currentUserService.Email ?? throw new UnauthorizedAccessException();

        var user = await _userRepository.GetByEmailAsync(userEmail, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        var emailToken = _tokenGenerator.GenerateSecureToken();

        var token = new EmailConfirmationToken(user.Id, emailToken, DateTime.UtcNow.AddMinutes(_emailConfirmationTokenSettings.ExpireMinutes));

        await _emailConfirmationTokenRepository.AddAsync(token, cancellationToken);

        var emailConfirmationLink = $"{_appSettings.FrontendUrl}/confirm-email?token={emailToken}";

        await _emailService.SendAsync(user.Email, EmailTemplate.EmailConfirmation, new EmailConfirmationModel { ConfirmationLink = emailConfirmationLink }, cancellationToken);

        return Unit.Value;
    }
}