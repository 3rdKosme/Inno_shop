using Inno_Shop.ProductService.Domain.Common;
using Inno_Shop.ProductService.Domain.Common.Constants;
using Inno_Shop.ProductService.Domain.Common.Exceptions;

namespace Inno_Shop.ProductService.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public double Price { get; private set; }
    public bool IsAvailable { get; private set; }
    public int UserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; private set; }

    private Product() { }

    public Product(string name, string description, int userId, double price)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Guard.AgainstNullOrWhiteSpace(description, nameof(description));
        Guard.AgainstNullOrNegative<int>(userId, nameof(userId));
        Guard.AgainstNullOrNegative<double>(price, nameof(price));
        
        Name = name;
        Description = description;
        IsAvailable = true;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
        Price = price;
    }

    public void ChangeName(string name)
    {
        Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Name = name;
    }

    public void ChangeDescription(string description) 
    {
        Guard.AgainstNullOrWhiteSpace(description, nameof(description));
        Description = description;
    }

    public void ChangePrice(double price)
    {
        Price = price;
    }

    public void SetAvailable()
    {
        if (IsAvailable) 
        {
            throw new AlreadyDoneException(ErrorMessages.AlreadyActivated);
        }
        IsAvailable = true;
    }

    public void SetUnavailable()
    {
        if (!IsAvailable)
        {
            throw new AlreadyDoneException(ErrorMessages.AlreadyDeactivated);
        }
        IsAvailable = false;
    }

    public void Delete()
    {
        if (IsDeleted)
        {
            throw new AlreadyDoneException(ErrorMessages.AlreadyDeleted);
        }
        IsDeleted = true;
    }

    public void Recover()
    {
        if (!IsDeleted)
        {
            throw new AlreadyDoneException(ErrorMessages.AlreadyRecovered);
        }
        IsDeleted = false;
    }
}