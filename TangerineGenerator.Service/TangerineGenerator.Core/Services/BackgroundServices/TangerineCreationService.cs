using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TangerineGenerator.Core.Commands;
using TangerineGenerator.Core.Models;

namespace TangerineGenerator.Core.Services.BackgroundServices;
    
internal class TangerineCreationService(
    ISender sender,
    ILogger<TangerineCreationService> logger,
    IOptions<TangerineGeneratorOptions> options)
    : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(options.Value.Delay));
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await sender.Send(new CreateTangerine.Command(), ct);
                await timer.WaitForNextTickAsync(ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while creating tangerine.");
            }
        }
    }
    
}