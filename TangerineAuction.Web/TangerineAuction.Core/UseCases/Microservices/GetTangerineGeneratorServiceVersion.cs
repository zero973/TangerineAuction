using Ardalis.Result;
using MediatR;
using TangerineAuction.Shared;
using TangerineAuction.Shared.Models;

namespace TangerineAuction.Core.UseCases.Microservices;

public class GetTangerineGeneratorServiceVersion
{
    public record Query() : IRequest<Result<VersionInfo>>;
}