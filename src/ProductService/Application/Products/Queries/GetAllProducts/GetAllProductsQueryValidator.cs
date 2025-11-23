using FluentValidation;
using Inno_Shop.ProductService.Application.Common.Constants;
using System.Security.Cryptography.X509Certificates;

namespace Inno_Shop.ProductService.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
{
    private HashSet<string> AllowedSortFields =
    [
        "name",
        "description",
        "price",
        "createdat",
        "userid"
    ];

    private HashSet<string> AllowedSortDirections =
    [
        "asc",
        "desc"
    ];

    public GetAllProductsQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100).WithMessage(ErrorMessages.QueryIsTooLong);

        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);

        RuleFor(x => x).Must(r => r.MaxPrice >= r.MinPrice).When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue).WithMessage(ErrorMessages.MinPriceGreaterThanMax);

        RuleFor(x => x.UserId).GreaterThanOrEqualTo(0).When(x => x.UserId.HasValue).WithMessage(ErrorMessages.UserIdMustBeGreaterThan0);

        

        RuleFor(x => x.Page).GreaterThan(0).WithMessage(ErrorMessages.);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        
    }

    private bool BeValidSortField(string sortBy)
    {
        return AllowedSortFields.Contains(sortBy.Trim().ToLowerInvariant());
    }

    private bool BeValidSortDirections(string direction) { 
        return AllowedSortDirections.Contains(direction.Trim().ToLowerInvariant());
    }
    
}