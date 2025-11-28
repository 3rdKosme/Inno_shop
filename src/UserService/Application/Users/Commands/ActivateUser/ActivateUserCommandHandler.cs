using MediatR;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.Shared.Application.Abstractions;
using Inno_Shop.Shared.Application.Exceptions;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Emails.Models;

namespace Inno_Shop.UserService.Application.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler(IUserRepository userRepository, IEmailService emailService, 
    IPasswordHasher passwordHasher, ICurrentUserService currentUserService, IProductServiceClient productServiceClient) 
    : IRequestHandler<ActivateUserCommand, Unit>
{
    public async Task<Unit> Handle(ActivateUserCommand request, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var user = await userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException(ErrorMessages.IncorrectPassword);
        }

        try
        {
            user.Activate();
        }
        catch (AlreadyDoneException ex)
        {
            throw new BusinessRuleValidationException(ex.Message);
        }

        await userRepository.UpdateAsync(user, cancellationToken);
        
        await productServiceClient.RecoverProductsAsync(user.Id, cancellationToken);

        await emailService.SendAsync(user.Email, EmailTemplate.Activated, new StatusChangedModel { Name = user.Name }, cancellationToken);

        return Unit.Value;
    }
}