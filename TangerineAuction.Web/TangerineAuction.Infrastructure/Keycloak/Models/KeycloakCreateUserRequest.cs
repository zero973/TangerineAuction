namespace TangerineAuction.Infrastructure.Keycloak.Models;

internal class KeycloakCreateUserRequest
{
    public string Username { get; init; } = null!;
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public bool Enabled { get; init; } = true;
    public bool EmailVerified { get; init; } = true;
}