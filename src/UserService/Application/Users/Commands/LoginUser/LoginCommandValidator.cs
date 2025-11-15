using MediatR;
using FluentValidation;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Commands.LoginUser;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorMessages.CurrentPasswordIsRequired).MinimumLength(8).WithMessage(ErrorMessages.PasswordMustBeAtLeast);

        RuleFor(x => x.Email).NotEmpty().WithMessage(ErrorMessages.EmailIsRequired).EmailAddress().WithMessage(ErrorMessages.IncorrectEmailFormat);
    }
}