using FluentAssertions;
using Inno_Shop.ProductService.Domain.Common.Exceptions;
using Inno_Shop.ProductService.Domain.Entities;

namespace Inno_Shop.ProductService.Domain.UnitTests.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_Should_Create_Product_When_Valid_Data()
    {
        string name = "Laptop";
        string description = "Gaming laptop";
        int userId = 1;
        double price = 2500;
        
        var product = new Product(name, description, userId, price);
        
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.UserId.Should().Be(userId);
        product.Price.Should().Be(price);
        product.IsAvailable.Should().BeTrue();
        product.IsDeleted.Should().BeFalse();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Throw_When_Name_Invalid(string invalidName)
    {
        Action act = () => new Product(invalidName, "desc", 1, 10);
        
        act.Should().Throw<DomainArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_Throw_When_Description_Invalid(string invalidDesc)
    {
        Action act = () => new Product("name", invalidDesc, 1, 10);
        
        act.Should().Throw<DomainArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_Should_Throw_When_UserId_Invalid(int invalidId)
    {
        Action act = () => new Product("name", "desc", invalidId, 10);
        
        act.Should().Throw<DomainArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10.5)]
    public void Constructor_Should_Throw_When_Price_Invalid(double invalidPrice)
    {
        Action act = () => new Product("name", "desc", 1, invalidPrice);
        
        act.Should().Throw<DomainArgumentException>();
    }
    
    [Fact]
    public void ChangeName_Should_Update_Name()
    {
        var product = new Product("Old", "desc", 1, 10);

        product.ChangeName("New");

        product.Name.Should().Be("New");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeName_Should_Throw_When_Invalid(string invalid)
    {
        var product = new Product("Old", "desc", 1, 10);

        Action act = () => product.ChangeName(invalid);

        act.Should().Throw<DomainArgumentException>();
    }

    [Fact]
    public void ChangeDescription_Should_Update_Description()
    {
        var product = new Product("name", "old", 1, 10);

        product.ChangeDescription("new");

        product.Description.Should().Be("new");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ChangeDescription_Should_Throw_When_Invalid(string invalid)
    {
        var product = new Product("name", "old", 1, 10);

        Action act = () => product.ChangeDescription(invalid);

        act.Should().Throw<DomainArgumentException>();
    }

    [Fact]
    public void ChangePrice_Should_Update_Price()
    {
        var product = new Product("name", "desc", 1, 10);

        product.ChangePrice(20.5);

        product.Price.Should().Be(20.5);
    }

    [Fact]
    public void SetAvailable_Should_Throw_When_Already_Available()
    {
        var product = new Product("name", "desc", 1, 10);

        Action act = () => product.SetAvailable();

        act.Should().Throw<AlreadyDoneException>();
    }

    [Fact]
    public void SetAvailable_Should_Set_Available_When_Unavailable()
    {
        var product = new Product("name", "desc", 1, 10);
        product.SetUnavailable();

        product.SetAvailable();

        product.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void SetUnavailable_Should_Throw_When_Already_Unavailable()
    {
        var product = new Product("name", "desc", 1, 10);
        product.SetUnavailable();

        Action act = () => product.SetUnavailable();

        act.Should().Throw<AlreadyDoneException>();
    }

    [Fact]
    public void SetUnavailable_Should_Set_Flag_When_Available()
    {
        var product = new Product("name", "desc", 1, 10);

        product.SetUnavailable();

        product.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void Delete_Should_Set_IsDeleted()
    {
        var product = new Product("name", "desc", 1, 10);

        product.Delete();

        product.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_Should_Throw_When_Already_Deleted()
    {
        var product = new Product("name", "desc", 1, 10);
        product.Delete();

        Action act = () => product.Delete();

        act.Should().Throw<AlreadyDoneException>();
    }

    [Fact]
    public void Recover_Should_Set_IsDeleted_False()
    {
        var product = new Product("name", "desc", 1, 10);
        product.Delete();

        product.Recover();

        product.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Recover_Should_Throw_When_Not_Deleted()
    {
        var product = new Product("name", "desc", 1, 10);

        Action act = () => product.Recover();

        act.Should().Throw<AlreadyDoneException>();
    }
}
