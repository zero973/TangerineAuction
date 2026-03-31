namespace TangerineAuction.Core.Services;

/// <summary>
/// Provides access to information about the currently authenticated user.
/// </summary>
public interface ICurrentUserService
{
    
    /// <summary>
    /// The unique identifier of the current user.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// The username of the current user.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Checks whether the current user has a specified role.
    /// </summary>
    /// <param name="role">The name of the role to check.</param>
    /// <returns>True if the user has the role; otherwise, false.</returns>
    public bool IsInRole(string role);

}