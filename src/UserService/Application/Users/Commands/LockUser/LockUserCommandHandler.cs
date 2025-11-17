using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using MediatR;

namespace Inno_Shop.UserService.Application.Users.Commands.LockUser;

public class LockUserCommandHandler(IUserRepository userRepository, 
    IEmailService emailService) : IRequestHandler<LockUserCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;

    public async Task<Unit> Handle(LockUserCommand request, CancellationToken cancellationToken = default)
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

        await _emailService.SendAsync(user.Email, EmailTemplate.Locked, new StatusChangedModel { Name = user.Name }, cancellationToken);

        return Unit.Value;
    }
}