using MediatR;
using FluentValidation;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Constants.ErrorMessages;

namespace Inno_Shop.UserService.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorMessages.CurrentPasswordIsRequired).MinimumLength(8).WithMessage(ErrorMessages.PasswordMustBeAtLeast);

        RuleFor(x => x.Name).MaximumLength(100).WithMessage(ErrorMessages.NameMustNotExceed);

        RuleFor(x => x.Email).EmailAddress().WithMessage(ErrorMessages.IncorrectEmailFormat);

        RuleFor(x => x.NewPassword).MinimumLength(8).WithMessage(ErrorMessages.PasswordMustBeAtLeast);
    }
}