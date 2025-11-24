using FluentValidation;
using Inno_Shop.ProductService.Application.Common.Constants;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ErrorMessages.IdIsRequired)
            .GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive);
    }
}