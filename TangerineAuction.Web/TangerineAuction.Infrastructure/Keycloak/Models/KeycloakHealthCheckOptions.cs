namespace TangerineAuction.Infrastructure.Keycloak.Models;

public class KeycloakHealthCheckOptions
{
    public string BaseUrl { get; set; } = "";
    public string HealthPath { get; set; } = "/health/ready";
}