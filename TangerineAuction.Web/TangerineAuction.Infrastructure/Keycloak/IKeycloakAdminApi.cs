using Refit;
using TangerineAuction.Infrastructure.Keycloak.Models;

namespace TangerineAuction.Infrastructure.Keycloak;

internal interface IKeycloakAdminApi
{
    
    [Post("/realms/{realm}/protocol/openid-connect/token")]
    Task<KeycloakTokenResponse> GetAccessTokenAsync(
        [AliasAs("realm")] string realm,
        [Body(BodySerializationMethod.UrlEncoded)] KeycloakTokenRequest request,
        CancellationToken ct = default);

    [Get("/admin/realms/{realm}/users")]
    Task<List<KeycloakUserDto>> SearchUsersAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("username")] string username,
        [AliasAs("exact")] bool exact,
        [AliasAs("max")] int max,
        [Header("Authorization")] string authorization,
        CancellationToken ct = default);

    [Get("/admin/realms/{realm}/users/{id}")]
    Task<KeycloakUserDto?> GetUserByIdAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("id")] string id,
        [Header("Authorization")] string authorization,
        CancellationToken ct = default);

    [Post("/admin/realms/{realm}/users")]
    Task<HttpResponseMessage> CreateUserAsync(
        [AliasAs("realm")] string realm,
        [Header("Authorization")] string authorization,
        [Body] KeycloakCreateUserRequest request,
        CancellationToken ct = default);
    
}