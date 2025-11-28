using FluentValidation;
using FluentValidation.Results;
using Inno_Shop.ProductService.Application.Common.Behaviors;
using MediatR;
using Moq;

namespace Inno_Shop.UserService.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    public record TestRequest(string Name) : IRequest<string>;

    private class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task Handle_NoValidators_Should_Call_Next()
    {
        var validators = new List<IValidator<TestRequest>>();

        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync("OK");

        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        var request = new TestRequest("test");

        var result = await behavior.Handle(request, nextMock.Object, CancellationToken.None);

        Assert.Equal("OK", result);
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Validators_NoErrors_Should_Call_Next()
    {
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<ValidationContext<TestRequest>>()))
            .Returns(new ValidationResult());

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };

        var nextMock = new Mock<RequestHandlerDelegate<string>>();
        nextMock.Setup(n => n(It.IsAny<CancellationToken>())).ReturnsAsync("OK");

        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        var request = new TestRequest("John");
        
        var result = await behavior.Handle(request, nextMock.Object, CancellationToken.None);
        
        Assert.Equal("OK", result);
        validatorMock.Verify(v => v.Validate(It.IsAny<ValidationContext<TestRequest>>()), Times.Once);
        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Validators_WithErrors_Should_Throw_ValidationException()
    {
        var failure = new ValidationFailure("Name", "Name is required");

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.Validate(It.IsAny<ValidationContext<TestRequest>>()))
            .Returns(new ValidationResult(new[] { failure }));

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };

        var nextMock = new Mock<RequestHandlerDelegate<string>>();

        var behavior = new ValidationBehavior<TestRequest, string>(validators);

        var request = new TestRequest("");
        
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(request, nextMock.Object, CancellationToken.None)
        );

        Assert.Contains(ex.Errors, e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required");

        validatorMock.Verify(v => v.Validate(It.IsAny<ValidationContext<TestRequest>>()), Times.Once);

        nextMock.Verify(n => n(It.IsAny<CancellationToken>()), Times.Never);
    }
}