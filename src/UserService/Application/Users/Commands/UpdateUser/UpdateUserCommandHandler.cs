using MediatR;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using System.ComponentModel.DataAnnotations;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Constants.ErrorMessages;

namespace Inno_Shop.UserService.Application.Users.Commands.AddUser;

public class UpdateUserCommandHandler(IUserRepository userRepository, IEmailService emailService, IPasswordHasher passwordHasher) : IRequestHandler<AddUserCommand, int>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<int> Handle(AddUserCommand request, CancellationToken cancellationToken = default)
    {
        if(await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new EmailAlreadyExistsException(ErrorMessages.EmailAlreadyExists);
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = User.Create(request.Name, request.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        //MAIL SENDING

        return user.Id;
    }
}