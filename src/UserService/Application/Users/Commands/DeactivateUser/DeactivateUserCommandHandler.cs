using MediatR;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using System.ComponentModel.DataAnnotations;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.UserService.Domain.Common.Exceptions;
using Inno_Shop.UserService.Application.Emails.Models;
using Inno_Shop.UserService.Application.Emails;

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

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null) {
            throw new DirectoryNotFoundException(ErrorMessages.UserNotFound);
        }

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

        await _userRepository.UpdateAsync(user);

        await _emailService.SendAsync(user.Email, EmailTemplate.Deactivated, new StatusChangedModel { Name = user.Name }, cancellationToken);

        return Unit.Value;
    }
}