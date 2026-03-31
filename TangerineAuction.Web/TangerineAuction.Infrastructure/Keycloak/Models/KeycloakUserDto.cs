namespace TangerineAuction.Infrastructure.Keycloak.Models;

internal class KeycloakUserDto
{
    
    public string Id { get; set; } = null!;
    
    public string Username { get; set; } = null!;
    
    public string? Email { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
    
    public bool? Enabled { get; set; }
    
    public Dictionary<string, string[]>? Attributes { get; set; }
    
}