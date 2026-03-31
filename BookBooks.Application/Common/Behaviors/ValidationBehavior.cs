using BookBooks.Domain.Common;
using FluentValidation;
using MediatR;

namespace BookBooks.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<FluentValidation.Results.ValidationFailure>();

        foreach (var validator in _validators)
        {
            var validationResult = await validator.ValidateAsync(context, cancellationToken);
            if (validationResult.Errors.Count > 0)
            {
                failures.AddRange(validationResult.Errors);
            }
        }

        if (failures.Count == 0)
        {
            return await next();
        }

        var message = string.Join("; ", failures.Select(x => x.ErrorMessage).Distinct());
        return CreateFailureResponse(message);
    }

    private static TResponse CreateFailureResponse(string errorMessage)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(errorMessage);
        }

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = typeof(TResponse).GetMethod(
                nameof(Result<object>.Failure),
                [typeof(string)]);

            if (failureMethod is not null)
            {
                return (TResponse)failureMethod.Invoke(null, [errorMessage])!;
            }
        }

        throw new ValidationException(errorMessage);
    }
}
