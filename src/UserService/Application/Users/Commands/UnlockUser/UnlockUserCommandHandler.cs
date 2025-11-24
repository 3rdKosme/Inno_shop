using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using MediatR;
using Inno_Shop.Shared.Application.Exceptions;

namespace Inno_Shop.UserService.Application.Users.Commands.UnlockUser;

public class UnlockUserCommandHandler(IUserRepository userRepository, 
    IEmailService emailService, IProductServiceClient productServiceClient) 
    : IRequestHandler<UnlockUserCommand, Unit>
{
    public async Task<Unit> Handle(UnlockUserCommand request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        try
        {
            user.Unlock();
        }
        catch (AlreadyDoneException ex)
        {
            throw new BusinessRuleValidationException(ex.Message);
        }

        await userRepository.UpdateAsync(user, cancellationToken);
        
        await productServiceClient.RecoverProductsAsync(user.Id, cancellationToken);

        await emailService.SendAsync(user.Email, EmailTemplate.Unlocked, new StatusChangedModel { Name = user.Name }, cancellationToken);

        return Unit.Value;
    }
}