using MediatR;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using System.ComponentModel.DataAnnotations;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Constants.ErrorMessages;
using Inno_Shop.Shared.Application.Exceptions;

namespace Inno_Shop.UserService.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserRepository userRepository, IEmailService emailService, IPasswordHasher passwordHasher) : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null) {
            throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        if(!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException(ErrorMessages.IncorrectPassword);
        }

        if(request.Name != null)
        {
            user.ChangeName(request.Name);
        }

        if (request.Email != null) 
        { 
            if(! await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                user.ChangeEmail(request.Email);
                //EMAIL SERVICE
            }
            else
            {
                throw new EmailAlreadyExistsException(ErrorMessages.EmailAlreadyExists);
            }
        }

        if (request.NewPassword != null)
        {
            user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        return Unit.Value;
    }
}