using Ardalis.Result;
using MediatR;

namespace TangerineAuction.Core.UseCases.Tangerines;

public class GenerateTangerine
{
    public record Command() : IRequest<Result>;
}