using BookBooks.Application.Common.Behaviors;
using BookBooks.Application.Features.Books.Commands;
using BookBooks.Domain.Common;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Tests.Common.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenValidationFails()
    {
        var validators = new List<IValidator<CreateBookCommand>>
        {
            new CreateBookCommandValidator()
        };

        var behavior = new ValidationBehavior<CreateBookCommand, Result<string>>(validators);
        var command = new CreateBookCommand("", "Author", "123", 0, null);

        var nextInvoked = false;
        RequestHandlerDelegate<Result<string>> next = _ =>
        {
            nextInvoked = true;
            return Task.FromResult(Result<string>.Success("ok"));
        };

        var result = await behavior.Handle(command, next, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
        Assert.False(nextInvoked);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationPasses()
    {
        var validators = new List<IValidator<CreateBookCommand>>
        {
            new CreateBookCommandValidator()
        };

        var behavior = new ValidationBehavior<CreateBookCommand, Result<string>>(validators);
        var command = new CreateBookCommand("Title", "Author", "1234567890", 2024, null);

        var nextInvoked = false;
        RequestHandlerDelegate<Result<string>> next = _ =>
        {
            nextInvoked = true;
            return Task.FromResult(Result<string>.Success("ok"));
        };

        var result = await behavior.Handle(command, next, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("ok", result.Value);
        Assert.True(nextInvoked);
    }
}
