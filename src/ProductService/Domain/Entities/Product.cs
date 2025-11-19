using Inno_Shop.ProductService.Domain.Common.Constants;
using Inno_Shop.ProductService.Domain.Common.Exceptions;

namespace Inno_Shop.ProductService.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsAvailable { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    private Product() { }

    public Product(string name, string description, int userId)
    {
        Name = name;
        Description = description;
        IsAvailable = true;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void ChangeName(string name)
    {
        Name = name;
    }

    public void ChangeDescription(string description) 
    { 
        Description = description;
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