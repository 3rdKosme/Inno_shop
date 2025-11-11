using FluentValidation;
using Inno_Shop.UserService.Application.Constants.ErrorMessages;

namespace Inno_Shop.UserService.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage(ErrorMessages.IdGreaterThan0);
    }
}