namespace TangerineAuction.Infrastructure.Keycloak.Models;

internal class SystemUserOptions
{
    
    public string Username { get; init; } = "system_user";
    
    public string Email { get; init; } = "system@tangerine.local";
    
    public string FirstName { get; init; } = "System";
    
    public string LastName { get; init; } = "User";
    
}