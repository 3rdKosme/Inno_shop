using MediatR;
using Inno_Shop.UserService.Application.Abstractions;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Emails;
using Inno_Shop.Shared.Application.Exceptions;

namespace Inno_Shop.UserService.Application.Users.Commands.DeactivateUser;

public class DeactivateUserCommandHandler(IUserRepository userRepository, IEmailService emailService, 
    IPasswordHasher passwordHasher, ICurrentUserService currentUserService) : IRequestHandler<DeactivateUserCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<Unit> Handle(DeactivateUserCommand request, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException(ErrorMessages.IncorrectPassword);
        }

        try
        {
            user.Deactivate();
        }
        catch (AlreadyDoneException ex)
        {
            throw new BusinessRuleValidationException(ex.Message);
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        await _emailService.SendAsync(user.Email, EmailTemplate.Deactivated, new StatusChangedModel { Name = user.Name }, cancellationToken);

        return Unit.Value;
    }
}