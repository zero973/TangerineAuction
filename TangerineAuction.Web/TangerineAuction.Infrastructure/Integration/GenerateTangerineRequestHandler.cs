using Ardalis.Result;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TangerineAuction.Core.UseCases.Tangerines;
using TangerineAuction.Infrastructure.Data;
using TangerineGenerator.Shared;

namespace TangerineAuction.Infrastructure.Integration;

internal class GenerateTangerineRequestHandler(
    IPublishEndpoint publishEndpoint, 
    AppDbContext dbContext,
    ILogger<GenerateTangerineRequestHandler> logger) 
    : IRequestHandler<GenerateTangerine.Command, Result>
{
    public async Task<Result> Handle(GenerateTangerine.Command request, CancellationToken ct)
    {
        try
        {
            await publishEndpoint.Publish(new GenerateTangerineRequest(), ct);
            await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Произошла ошибка при генерации мандаринки");
            return Result.Error(e.Message);
        }
        
        return Result.Success();
    }
}