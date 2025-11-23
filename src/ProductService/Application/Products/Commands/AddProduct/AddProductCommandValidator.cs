using FluentValidation;
using Inno_Shop.ProductService.Application.Common.Constants;

namespace Inno_Shop.ProductService.Application.Products.Commands.AddProduct;

public class GetProductsQueryValidator : AbstractValidator<AddProductCommand>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ErrorMessages.NameIsRequired).MaximumLength(100).WithMessage(ErrorMessages.NameMustNotExceed);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ErrorMessages.DescriptionIsRequired).MaximumLength(500).WithMessage(ErrorMessages.DescriptionMustNotExceed);
        RuleFor(x => x.Price).NotEmpty().WithMessage(ErrorMessages.PriceIsRequired).GreaterThan(0).WithMessage(ErrorMessages.PriceMustBePositive);
        RuleFor(x => x.UserId).NotEmpty().WithMessage(ErrorMessages.UserIdIsRequired).GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive);
    }
}