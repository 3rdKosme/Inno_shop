using FluentValidation;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Microsoft.Extensions.Options;
using MediatR;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Domain.Common.Exceptions;

namespace Inno_Shop.UserService.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler(IUserRepository userRepository, IEmailConfirmationTokenRepository emailConfirmationTokenRepository) : IRequestHandler<ConfirmEmailCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailConfirmationTokenRepository _emailConfirmationTokenRepository = emailConfirmationTokenRepository;

    public async Task<Unit> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken = default)
    {
        var stored = await _emailConfirmationTokenRepository.GetByTokenAsync(request.Token, cancellationToken) ?? throw new InvalidCredentialsException(ErrorMessages.IncorrectToken);

        if(stored.IsExpired || stored.IsRevoked)
        {
            throw new TokenIsExpiredOrRevokedException(ErrorMessages.TokenIsExpiredOrRevoked);
        }

        var user = await _userRepository.GetByIdAsync(stored.UserId, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        try
        {
            user.ConfirmEmail();
            stored.Revoke();
        }
        catch (EmailAlreadyConfirmedException ex) 
        {
            throw new BusinessRuleValidationException(ex.Message);
        }

        
        
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _emailConfirmationTokenRepository.UpdateAsync(stored, cancellationToken);

        return Unit.Value;
    }
}