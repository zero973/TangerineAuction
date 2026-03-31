using Ardalis.Result;
using MediatR;
using TangerineAuction.Shared;

namespace TangerineAuction.Core.UseCases.Microservices;

public class GetTangerineGeneratorServiceVersion
{
    public record Query() : IRequest<Result<VersionInfo>>;
}