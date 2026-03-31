using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TangerineAuction.Infrastructure.Keycloak.Models;

namespace TangerineAuction.Server.HealthChecks;

internal class KeycloakCheck(IHttpClientFactory httpFactory, IOptions<KeycloakHealthCheckOptions> options) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, options.Value.HealthPath);

            var client = httpFactory.CreateClient();
            client.BaseAddress = new Uri(options.Value.BaseUrl);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

            return !response.IsSuccessStatusCode
                ? new HealthCheckResult(
                    status: HealthStatus.Unhealthy,
                    description: $"Keycloak returned {(int)response.StatusCode} {response.ReasonPhrase}.")
                : new HealthCheckResult(status: HealthStatus.Healthy, description: "Keycloak is healthy");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(
                status: HealthStatus.Unhealthy,
                description: "Keycloak check threw an exception",
                exception: ex);
        }
    }
}