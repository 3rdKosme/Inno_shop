using FluentValidation;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage(ErrorMessages.TokenIsRequired);

        RuleFor(x => x.NewPassword).NotEmpty().WithMessage(ErrorMessages.PasswordIsRequired).MinimumLength(8).WithMessage(ErrorMessages.PasswordMustBeAtLeast);
    }
}