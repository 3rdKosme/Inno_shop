using FluentValidation;
using Inno_Shop.ProductService.Application.Common.Constants;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetProducts;

public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    private readonly HashSet<string> _allowedSorts =
    [
        "price_asc",
        "price_desc",
        "created_asc",
        "created_desc"
    ];

    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100).WithMessage(ErrorMessages.QueryIsTooLong);

        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);

        RuleFor(x => x).Must(r => r.MaxPrice >= r.MinPrice).When(x =>
            x.MinPrice.HasValue && x.MaxPrice.HasValue).WithMessage(ErrorMessages.MinPriceGreaterThanMax);

        RuleFor(x => x.UserId).GreaterThanOrEqualTo(0).When(x => x.UserId.HasValue)
            .WithMessage(ErrorMessages.IdMustBePositive);

        RuleFor(x => x.Sort).Must(x => x is null || _allowedSorts.Contains(x))
            .WithMessage(ErrorMessages.InvalidSortType);

        RuleFor(x => x.Page).GreaterThan(0).WithMessage(ErrorMessages.PageMustBePositive);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage(ErrorMessages.PageSizeMustBeBetween);
        
    }
}