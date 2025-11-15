using MediatR;
using FluentValidation;
using Inno_Shop.UserService.Application.DTOs;
using Inno_Shop.UserService.Application.Common.Constants;

namespace Inno_Shop.UserService.Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(ErrorMessages.RefreshTokenIsRequired);

    }
}