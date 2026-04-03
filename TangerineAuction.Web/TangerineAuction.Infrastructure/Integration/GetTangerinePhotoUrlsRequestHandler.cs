using Ardalis.Result;
using MediatR;
using TangerineAuction.Core.UseCases.Tangerines;
using TangerineGenerator.Shared;

namespace TangerineAuction.Infrastructure.Integration;

internal class GetTangerinePhotoUrlsRequestHandler(ITangerineGeneratorService generatorService) 
    : IRequestHandler<GetTangerinePhotoUrls.Query, Result<Dictionary<string, string>>>
{
    public async Task<Result<Dictionary<string, string>>> Handle(GetTangerinePhotoUrls.Query request, CancellationToken ct) 
        => await generatorService.GetPhotoUrls(request.TangerineFileNames, ct);
}