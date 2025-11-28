using FluentValidation;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Commands.SendPasswordResetCode;

public class SendPasswordResetCodeCommandValidator : AbstractValidator<SendPasswordResetCodeCommand>
{
    public SendPasswordResetCodeCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage(ErrorMessages.EmailIsRequired).EmailAddress().WithMessage(ErrorMessages.IncorrectEmailFormat);
    }
}