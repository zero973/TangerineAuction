using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace TangerineAuction.Core.Behaviors;

internal class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var validationResults = new List<ValidationResult>();
        foreach (var validator in validators)
            validationResults.Add(await validator.ValidateAsync(context, cancellationToken));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var errorList = new ErrorList(failures.Select(f => f.ErrorMessage));

        if (!typeof(TResponse).IsGenericType || typeof(TResponse).GetGenericTypeDefinition() != typeof(Result<>))
            return (TResponse)(object)Result.Error(errorList);
        
        var genericType = typeof(TResponse).GetGenericArguments()[0];

        var genericErrorMethod = typeof(Result<>)
            .MakeGenericType(genericType)
            .GetMethod(nameof(Result<>.Error), [typeof(ErrorList)]);

        var genericErrorResult = genericErrorMethod?.Invoke(null, [errorList]);

        return (TResponse)genericErrorResult!;

    }
}