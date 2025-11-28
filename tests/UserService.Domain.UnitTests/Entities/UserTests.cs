using Inno_Shop.UserService.Domain.Common.Exceptions;
using Inno_Shop.UserService.Domain.Entities;
using Inno_Shop.UserService.Domain.Enums;

namespace Inno_Shop.UserService.Domain.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void Create_Should_Create_User_With_Default_Values()
    {
        var user = User.Create("John", "john@mail.com", "hash");

        Assert.Equal("John", user.Name);
        Assert.Equal("john@mail.com", user.Email);
        Assert.Equal("hash", user.PasswordHash);
        Assert.Equal(UserRole.User, user.UserRole);
        Assert.False(user.IsEmailConfirmed);
        Assert.True(user.IsActive);
        Assert.False(user.IsLocked);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Throw_When_Name_Invalid(string name)
    {
        Assert.Throws<DomainArgumentNullException>(() =>
            User.Create(name, "mail@mail.com", "hash"));
    }
    
    [Fact]
    public void ChangeName_Should_Update_Name()
    {
        var user = User.Create("Old", "mail@mail.com", "hash");

        user.ChangeName("New");

        Assert.Equal("New", user.Name);
    }

    [Theory]
    [InlineData("")]
    public void ChangeName_Should_Throw_When_Invalid(string newName)
    {
        var user = User.Create("Old", "mail@mail.com", "hash");

        Assert.Throws<DomainArgumentNullException>(() => user.ChangeName(newName));
    }
    
    [Fact]
    public void ChangeEmail_Should_Update_And_Reset_Confirmation()
    {
        var user = User.Create("John", "old@mail.com", "hash");
        user.ConfirmEmail();

        user.ChangeEmail("new@mail.com");

        Assert.Equal("new@mail.com", user.Email);
        Assert.False(user.IsEmailConfirmed);
    }
    
    [Fact]
    public void PromoteToAdmin_Should_Set_Role()
    {
        var user = User.Create("John", "a@a.com", "hash");

        user.PromoteToAdmin();

        Assert.Equal(UserRole.Admin, user.UserRole);
    }

    [Fact]
    public void PromoteToAdmin_Should_Throw_When_Already_Admin()
    {
        var user = User.Create("John", "a@a.com", "hash");
        user.PromoteToAdmin();

        Assert.Throws<AlreadyDoneException>(() => user.PromoteToAdmin());
    }
    
    [Fact]
    public void ConfirmEmail_Should_Set_Flag()
    {
        var user = User.Create("John", "a@a.com", "hash");

        user.ConfirmEmail();

        Assert.True(user.IsEmailConfirmed);
    }

    [Fact]
    public void ConfirmEmail_Should_Throw_When_Already_Confirmed()
    {
        var user = User.Create("John", "a@a.com", "hash");
        user.ConfirmEmail();

        Assert.Throws<EmailAlreadyConfirmedException>(() => user.ConfirmEmail());
    }
    
    [Fact]
    public void Deactivate_Should_Deactivate_User()
    {
        var user = User.Create("John", "a@a.com", "hash");

        user.Deactivate();

        Assert.False(user.IsActive);
    }

    [Fact]
    public void Deactivate_Should_Throw_When_Already_Inactive()
    {
        var user = User.Create("John", "a@a.com", "hash");
        user.Deactivate();

        Assert.Throws<AlreadyDoneException>(() => user.Deactivate());
    }
    
    [Fact]
    public void Lock_Should_Set_IsLocked()
    {
        var user = User.Create("John", "a@a.com", "hash");

        user.Lock();

        Assert.True(user.IsLocked);
    }

    [Fact]
    public void Unlock_Should_Throw_When_Not_Locked()
    {
        var user = User.Create("John", "a@a.com", "hash");

        Assert.Throws<AlreadyDoneException>(() => user.Unlock());
    }
    
}