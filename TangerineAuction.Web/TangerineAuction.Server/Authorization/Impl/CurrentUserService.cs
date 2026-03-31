using System.Security.Claims;
using TangerineAuction.Core.Services;

namespace TangerineAuction.Server.Authorization.Impl;

/// <summary>
/// Retrieves user identity and role information from the current HTTP context.
/// </summary>
internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{

    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;
    
    private IEnumerable<string> Roles =>
        httpContextAccessor.HttpContext?.User.FindAll("role").Select(c => c.Value) ?? [];
    
    public Guid UserId
    {
        get
        {
            var stringUserId = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(stringUserId, out var userId);
            return userId;
        }
    }

    public string? UserName => Principal?.FindFirstValue("preferred_username");

    public bool IsInRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    
}