using FluentValidation;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Microsoft.Extensions.Options;
using MediatR;
using Inno_Shop.UserService.Application.Common.Settings;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Exceptions;

namespace Inno_Shop.UserService.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IUserRepository userRepository, IPasswordResetTokenRepository passwordResetTokenRepository,
    IPasswordHasher passwordHasher) : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordResetTokenRepository _passwordTokenRepository = passwordResetTokenRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken = default)
    {
        var stored = await _passwordTokenRepository.GetByTokenAsync(request.Token, cancellationToken) ?? throw new InvalidCredentialsException(ErrorMessages.IncorrectToken);

        if(stored.IsExpired || stored.IsRevoked)
        {
            throw new TokenIsExpiredOrRevokedException(ErrorMessages.TokenIsExpiredOrRevoked);
        }

        var user = await _userRepository.GetByIdAsync(stored.UserId, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));

        await _userRepository.UpdateAsync(user, cancellationToken);       

        return Unit.Value;
    }
}