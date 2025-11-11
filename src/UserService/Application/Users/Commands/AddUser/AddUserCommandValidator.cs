using MediatR;
using FluentValidation;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Constants.ErrorMessages;

namespace Inno_Shop.UserService.Application.Users.Commands.AddUser;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ErrorMessages.NameIsRequired).MaximumLength(100).WithMessage(ErrorMessages.NameMustNotExceed);

        RuleFor(x => x.Email).NotEmpty().WithMessage(ErrorMessages.EmailIsRequired).EmailAddress().WithMessage(ErrorMessages.IncorrectEmailFormat);

        RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorMessages.PasswordIsRequired).MinimumLength(8).WithMessage(ErrorMessages.PasswordMustBeAtLeast);
    }
}