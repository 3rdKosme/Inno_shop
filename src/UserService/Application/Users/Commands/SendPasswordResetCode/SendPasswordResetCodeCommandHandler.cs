using FluentValidation;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Microsoft.Extensions.Options;
using MediatR;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Domain.Entities;

namespace Inno_Shop.UserService.Application.Users.Commands.SendPasswordResetCode;

public class ResetPasswordCommandHandler(IUserRepository userRepository, IEmailService emailService, 
    IPasswordResetTokenRepository passwordResetTokenRepository, IOptions<AppSettings> appSettings,
    IOptions<PasswordResetTokenSettings> passwordResetTokenSettings, 
    ITokenGenerator tokenGenerator) : IRequestHandler<SendPasswordResetCodeCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordResetTokenRepository _passwordTokenRepository = passwordResetTokenRepository;
    private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
    private readonly PasswordResetTokenSettings _passwordResetTokenSettings = passwordResetTokenSettings.Value;
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<Unit> Handle(SendPasswordResetCodeCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        var token = _tokenGenerator.GenerateSecureToken();

        var resetToken = new PasswordResetToken(user.Id, token, DateTime.UtcNow.AddMinutes(_passwordResetTokenSettings.ExpireMinutes));

        await _passwordTokenRepository.AddAsync(resetToken, cancellationToken);

        var resetLink = $"{_appSettings.FrontendUrl}/reset-password?token={token}";
        await _emailService.SendPasswordResetLinkAsync(user.Email, resetLink);

        return Unit.Value;
    }
}