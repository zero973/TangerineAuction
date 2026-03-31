using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using TangerineAuction.Core.Services;
using TangerineAuction.Infrastructure.Keycloak.Models;

namespace TangerineAuction.Infrastructure.Keycloak;

internal class SystemUserService(
    IKeycloakAdminApi api,
    IKeycloakTokenService tokenService,
    IOptions<KeycloakOptions> keycloakOptions,
    IOptions<SystemUserOptions> systemUserOptions,
    ILogger<SystemUserService> logger)
    : ISystemUserService, IHostedService
{
    
    private readonly SemaphoreSlim _sync = new(1, 1);
    private KeycloakUserDto? _user;

    public Guid UserId
    {
        get
        {
            if (!Guid.TryParse(_user?.Id, out Guid id))
                throw new InvalidOperationException("System user is not initialized.");
            return id;
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await EnsureInitialized(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task EnsureInitialized(CancellationToken ct = default)
    {
        if (_user is not null)
            return;

        await _sync.WaitAsync(ct);
        try
        {
            if (_user is not null)
                return;

            var realm = keycloakOptions.Value.Realm;
            var token = await tokenService.GetAccessToken(ct);
            var auth = $"Bearer {token}";

            var cfg = systemUserOptions.Value;

            logger.LogInformation($"Пытаюсь получить системного пользователя");
            
            var existingUsers = await api.SearchUsersAsync(realm, cfg.Username, exact: true, max: 1, auth, ct);

            var existing = existingUsers.FirstOrDefault();
            if (existing is not null)
            {
                _user = existing;
                logger.LogInformation($"Найден системный пользователь. Id: {existing.Id}");
                return;
            }

            logger.LogInformation($"Создаю системного пользователя");
            
            var createResponse = await api.CreateUserAsync(
                realm,
                auth,
                new KeycloakCreateUserRequest
                {
                    Username = cfg.Username,
                    Email = cfg.Email,
                    FirstName = cfg.FirstName,
                    LastName = cfg.LastName,
                    Enabled = true,
                    EmailVerified = true
                },
                ct);

            if (createResponse.StatusCode != HttpStatusCode.Created &&
                createResponse.StatusCode != HttpStatusCode.NoContent)
            {
                throw new InvalidOperationException(
                    $"Failed to create system user in Keycloak. Status code: {createResponse.StatusCode}");
            }

            var createdId = createResponse.Headers.Location!.Segments.Last().Trim('/');
            
            var created = await api.GetUserByIdAsync(realm, createdId, auth, ct);
            _user = created ?? new KeycloakUserDto
            {
                Id = createdId,
                Username = cfg.Username,
                Email = cfg.Email,
                FirstName = cfg.FirstName,
                LastName = cfg.LastName
            };

            logger.LogInformation($"Системный пользователь успешно создан, Id = {_user.Id}");
        }
        finally
        {
            _sync.Release();
        }
    }

}