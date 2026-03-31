using MediatR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TangerineAuction.Core.UseCases.Microservices;

namespace TangerineAuction.Server.HealthChecks;

internal class TangerineGeneratorServiceCheck(ISender sender) : IHealthCheck
{
    
    private const string CheckName = "Tangerine generator service";
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        
        try
        {
            var result = await sender.Send(new GetTangerineGeneratorServiceVersion.Query(), ct);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            return result != null ? 
                new HealthCheckResult(status: HealthStatus.Healthy, description: $"{CheckName} is healthy") 
                : new HealthCheckResult(status: HealthStatus.Unhealthy, description: $"{CheckName} check didn't pass");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(
                status: HealthStatus.Unhealthy,
                description: $"{CheckName} check threw an exception",
                exception: ex);
        }
    }
}