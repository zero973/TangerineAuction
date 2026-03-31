using Ardalis.Result;
using FluentValidation;
using MediatR;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.UseCases.Tangerines.Dtos;

namespace TangerineAuction.Core.UseCases.Tangerines;

/// <summary>
/// Сохранение мандарина в БД
/// </summary>
public class AddTangerine
{
    
    public record Command(TangerineRequest Request) : IRequest<Result<TangerineResponse>>;
    
    internal class RequestHandler(IRepository<Tangerine> repository) : IRequestHandler<Command, Result<TangerineResponse>>
    {
        public async Task<Result<TangerineResponse>> Handle(Command request, CancellationToken ct)
        {
            var report = Tangerine.Create(request.Request.Name, request.Request.Quality, request.Request.StartPrice, 
                request.Request.FilePath);
            var result = await repository.AddAsync(report, ct);
            return new TangerineResponse(result);
        }
    }
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Request).SetValidator(new TangerineRequestValidator());
        }
    }
    
}