using System.Security.Claims;
using FluentAssertions;
using Inno_Shop.Shared.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Inno_Shop.Shared.Infrastructure.UnitTests.Services;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    
    private CurrentUserService CreateService()
    {
        return new CurrentUserService(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void UserId_Should_Return_Value_When_Claim_Valid_Int()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "42") };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var sut = CreateService();
        
        var result = sut.UserId;
        
        result.Should().Be(42);
    }

    [Fact]
    public void UserId_Should_Return_Null_When_Claim_Is_Not_Int()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "not-an-int") };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var sut = CreateService();
        
        var result = sut.UserId;
        
        result.Should().BeNull();
    }

    [Fact]
    public void UserId_Should_Return_Null_When_Claim_Missing()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var httpContext = new DefaultHttpContext { User = user };

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var sut = CreateService();
        
        var result = sut.UserId;
        
        result.Should().BeNull();
    }

    [Fact]
    public void UserId_Should_Return_Null_When_User_Is_Null()
    {
        var httpContext = new DefaultHttpContext { User = null! };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var sut = CreateService();
        
        var result = sut.UserId;
        
        result.Should().BeNull();
    }

    [Fact]
    public void UserId_Should_Return_Null_When_HttpContext_Is_Null()
    {
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var sut = CreateService();
        
        var result = sut.UserId;
        
        result.Should().BeNull();
    }

    [Fact]
    public void Email_Should_Return_Email_When_Claim_Present()
    {
        var claims = new[] { new Claim(ClaimTypes.Email, "user@test.com") };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = user };

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var sut = CreateService();
        
        var result = sut.Email;
        
        result.Should().Be("user@test.com");
    }

    [Fact]
    public void Email_Should_Return_Null_When_Claim_Missing()
    {
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var sut = CreateService();
        
        var result = sut.Email;
        
        result.Should().BeNull();
    }

    [Fact]
    public void Email_Should_Return_Null_When_User_Is_Null()
    {
        var httpContext = new DefaultHttpContext { User = null! };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        var sut = CreateService();
        
        var result = sut.Email;
        
        result.Should().BeNull();
    }

    [Fact]
    public void Email_Should_Return_Null_When_HttpContext_Is_Null()
    {
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(default(HttpContext?));

        var sut = CreateService();
        
        var result = sut.Email;
        
        result.Should().BeNull();
    }
}