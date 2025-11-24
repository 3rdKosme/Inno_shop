using FluentValidation;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Microsoft.Extensions.Options;
using MediatR;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using Inno_Shop.UserService.Domain.Entities;

namespace Inno_Shop.UserService.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler(IUserRepository userRepository, ITokenRepository<EmailConfirmationToken> emailConfirmationTokenRepository) : IRequestHandler<ConfirmEmailCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken = default)
    {
        var stored = await emailConfirmationTokenRepository.GetByTokenAsync(request.Token, cancellationToken) 
                     ?? throw new InvalidCredentialsException(ErrorMessages.IncorrectToken);

        if(stored.IsExpired || stored.IsRevoked)
        {
            throw new TokenIsExpiredOrRevokedException(ErrorMessages.TokenIsExpiredOrRevoked);
        }

        var user = await userRepository.GetByIdAsync(stored.UserId, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        try
        {
            user.ConfirmEmail();
            stored.Revoke();
        }
        catch (EmailAlreadyConfirmedException ex) 
        {
            throw new BusinessRuleValidationException(ex.Message);
        }

        
        
        await userRepository.UpdateAsync(user, cancellationToken);
        await emailConfirmationTokenRepository.UpdateAsync(stored, cancellationToken);

        return Unit.Value;
    }
}