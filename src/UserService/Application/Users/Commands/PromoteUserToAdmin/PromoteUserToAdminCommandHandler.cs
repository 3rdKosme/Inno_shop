using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using MediatR;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.Shared.Application.Exceptions;

namespace Inno_Shop.UserService.Application.Users.Commands.PromoteUserToAdmin;

public class PromoteUserToAdminCommandHandler(IUserRepository userRepository) 
    : IRequestHandler<PromoteUserToAdminCommand, Unit>
{
    public async Task<Unit> Handle(PromoteUserToAdminCommand request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        try
        {
            user.PromoteToAdmin();
        }
        catch (AlreadyDoneException ex)
        {
            throw new BusinessRuleValidationException(ex.Message);
        }

        await userRepository.UpdateAsync(user, cancellationToken);

        return Unit.Value;
    }
}