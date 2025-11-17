using MediatR;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Application.Abstractions;
using System.ComponentModel.DataAnnotations;
using Inno_Shop.UserService.Application.Exceptions;
using Inno_Shop.UserService.Application.Common.Constants;
using Inno_Shop.Shared.Application.Exceptions;

namespace Inno_Shop.UserService.Application.Users.Commands.UpdateUserAdmin;

public class UpdateUserAdminCommandHandler(IUserRepository userRepository, IEmailService emailService)
    : IRequestHandler<UpdateUserAdminCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    public async Task<Unit> Handle(UpdateUserAdminCommand request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException(ErrorMessages.UserNotFound);

        if(request.Name != null)
        {
            user.ChangeName(request.Name);
        }

        await _userRepository.UpdateAsync(user, cancellationToken);
        //EMAIL SERVICE

        return Unit.Value;
    }
}