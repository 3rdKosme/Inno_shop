using FluentValidation;
using Inno_Shop.ProductService.Application.Common.Constants;

namespace Inno_Shop.ProductService.Application.Products.Commands.UpdateProductAdmin;

public class UpdateProductAdminCommandValidator : AbstractValidator<UpdateProductAdminCommand>
{
    public UpdateProductAdminCommandValidator()
    {
        RuleFor(x => x.Name).MaximumLength(100).WithMessage(ErrorMessages.NameMustNotExceed);
        RuleFor(x => x.Description).MaximumLength(500).WithMessage(ErrorMessages.DescriptionMustNotExceed);
        RuleFor(x => x.Id).NotEmpty().WithMessage(ErrorMessages.IdIsRequired).GreaterThan(0).WithMessage(ErrorMessages.IdMustBePositive);
    }
}