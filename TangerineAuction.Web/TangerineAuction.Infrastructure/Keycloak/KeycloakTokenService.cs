using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TangerineAuction.Infrastructure.Keycloak.Models;

namespace TangerineAuction.Infrastructure.Keycloak;

internal class KeycloakTokenService(IKeycloakAdminApi api, IOptions<KeycloakOptions> options, ILogger<KeycloakTokenService> logger)
    : IKeycloakTokenService, IHostedService
{
    
    private readonly KeycloakOptions _options = options.Value;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private KeycloakTokenResponse? _cachedToken;
    private DateTimeOffset _expiresAtUtc;

    public Task StartAsync(CancellationToken cancellationToken) => EnsureToken(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task<string> GetAccessToken(CancellationToken ct = default)
    {
        if (IsTokenValid())
            return _cachedToken!.AccessToken;

        await _lock.WaitAsync(ct);
        try
        {
            if (!IsTokenValid())
                await RefreshToken(ct);

            return _cachedToken!.AccessToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool IsTokenValid()
    {
        if (_cachedToken is null)
            return false;

        return DateTimeOffset.UtcNow < _expiresAtUtc - TimeSpan.FromSeconds(30);
    }

    private async Task EnsureToken(CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            if (!IsTokenValid())
                await RefreshToken(ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task RefreshToken(CancellationToken ct)
    {
        logger.LogInformation("Запрашиваю AccessToken");
        
        var response = await api.GetAccessTokenAsync(
            _options.Realm,
            new KeycloakTokenRequest
            {
                ClientId = _options.Resource,
                ClientSecret = _options.Credentials.Secret,
                GrantType = "client_credentials"
            },
            ct);

        _cachedToken = response;
        _expiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn);
        
        logger.LogInformation($"AccessToken получен. Истекает: {_expiresAtUtc}");
    }
    
}