using MediatR;
using TangerineAuction.Core.UseCases.Microservices;
using TangerineGenerator.Shared;

namespace TangerineAuction.Infrastructure.Integration;

internal class DeleteTangerineImageHandler(ITangerineGeneratorService generatorService) 
    : IRequestHandler<DeleteTangerineImage.Command>
{
    public Task Handle(DeleteTangerineImage.Command request, CancellationToken ct)
        => generatorService.DeleteImage(request.FilePath).AsTask();
}