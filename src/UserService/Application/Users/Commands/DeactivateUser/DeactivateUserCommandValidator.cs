using MediatR;
using FluentValidation;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Commands.DeactivateUser;

public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
{
    public DeactivateUserCommandValidator()
    {
        RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorMessages.PasswordIsRequired);
    }
}