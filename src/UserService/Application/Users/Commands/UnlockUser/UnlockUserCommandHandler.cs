using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.UnlockUser;

public class UnlockUserCommandHandler(IUserRepository userRepository, 
    IEmailService emailService) : IRequestHandler<UnlockUserCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;

    public async Task<Unit> Handle(UnlockUserCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new DirectoryNotFoundException(ErrorMessages.UserNotFound);

        try
        {
            user.Lock();
        }
        catch (AlreadyDoneException ex)
        {
            throw new BusinessRuleValidationException(ex.Message);
        }

        await _userRepository.UpdateAsync(user);

        //MAIL SENDING

        return Unit.Value;
    }
}