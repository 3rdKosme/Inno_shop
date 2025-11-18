using FluentValidation;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Commands.UpdateUserAdmin;

public class UpdateUserAdminCommandValidator : AbstractValidator<UpdateUserAdminCommand>
{
    public UpdateUserAdminCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage(ErrorMessages.IdGreaterThan0);

        RuleFor(x => x.Name).MaximumLength(100).WithMessage(ErrorMessages.NameMustNotExceed);
    }
}