using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using MediatR;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Domain.Common.Exceptions;

namespace Inno_Shop.UserService.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IUserRepository userRepository, ITokenRepository<PasswordResetToken> passwordResetTokenRepository,
    IPasswordHasher passwordHasher) : IRequestHandler<ResetPasswordCommand, Unit>
{
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken = default)
    {
        var stored = await passwordResetTokenRepository.GetByTokenAsync(request.Token, cancellationToken) 
                     ?? throw new InvalidCredentialsException(ErrorMessages.IncorrectToken);

        if(stored.IsExpired || stored.IsRevoked)
        {
            throw new TokenIsExpiredOrRevokedException(ErrorMessages.TokenIsExpiredOrRevoked);
        }

        var user = await userRepository.GetByIdAsync(stored.UserId, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        try
        {
            user.ChangePassword(passwordHasher.HashPassword(request.NewPassword));
            stored.Revoke();
        }
        catch (DomainArgumentNullException ex)
        {
            throw new BusinessRuleValidationException(ex.Message);
        }
       
        await userRepository.UpdateAsync(user, cancellationToken);       
        await passwordResetTokenRepository.UpdateAsync(stored, cancellationToken);

        return Unit.Value;
    }
}