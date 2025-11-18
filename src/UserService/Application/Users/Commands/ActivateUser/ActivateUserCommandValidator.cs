using FluentValidation;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Commands.ActivateUser;

public class ActivateUserCommandValidator : AbstractValidator<ActivateUserCommand>
{
    public ActivateUserCommandValidator()
    {
        RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorMessages.PasswordIsRequired);
    }
}