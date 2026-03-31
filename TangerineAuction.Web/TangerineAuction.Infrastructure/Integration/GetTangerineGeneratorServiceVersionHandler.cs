using Ardalis.Result;
using MediatR;
using TangerineAuction.Core.UseCases.Microservices;
using TangerineAuction.Shared;
using TangerineGenerator.Shared;

namespace TangerineAuction.Infrastructure.Integration;

internal class GetTangerineGeneratorServiceVersionHandler(ITangerineGeneratorService generatorService) 
    : IRequestHandler<GetTangerineGeneratorServiceVersion.Query, Result<VersionInfo>>
{
    public async Task<Result<VersionInfo>> Handle(GetTangerineGeneratorServiceVersion.Query request, CancellationToken ct) 
        => await generatorService.GetVersion();
}