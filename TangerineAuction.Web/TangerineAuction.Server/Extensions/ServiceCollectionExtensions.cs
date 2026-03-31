using Microsoft.OpenApi;
using NLog;
using NLog.Web;
using RabbitMQ.Client;
using TangerineAuction.Infrastructure.Keycloak.Models;
using TangerineAuction.Server.HealthChecks;

namespace TangerineAuction.Server.Extensions;

internal static class ServiceCollectionExtensions
{

    public static void AddLogger(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        LogManager.Setup().LoadConfigurationFromAppSettings();
        builder.Host.UseNLog();
    }
    
    public static void AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            var realm = configuration["Keycloak:realm"]!;
            var authServer = configuration["Keycloak:auth-server-url"]!.TrimEnd('/');

            var authorizationUrl = new Uri($"{authServer}/realms/{realm}/protocol/openid-connect/auth");

            var tokenUrl = new Uri($"{authServer}/realms/{realm}/protocol/openid-connect/token");

            var scopes = new Dictionary<string, string>
            {
                ["openid"] = "openid",
                ["profile"] = "profile",
                ["email"] = "email"
            };

            o.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "Keycloak Authorization Code flow with PKCE",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = authorizationUrl,
                        TokenUrl = tokenUrl,
                        Scopes = scopes
                    }
                }
            });

            o.AddSecurityRequirement(doc =>
            {
                var schemeRef = new OpenApiSecuritySchemeReference("Keycloak", doc);
                var requirement = new OpenApiSecurityRequirement { [ schemeRef ] = new List<string>() };
                return requirement;
            });

        });
    }

    public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakHealthCheckOptions>(configuration.GetSection("Keycloak:Health"));
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Postgres")!)
            .AddRedis(configuration.GetConnectionString("Redis")!)
            .AddRabbitMQ(x =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:Host"]!,
                    UserName = configuration["RabbitMQ:User"]!,
                    Password = configuration["RabbitMQ:Password"]!,
                    Port = AmqpTcpEndpoint.UseDefaultPort
                };
                return factory.CreateConnectionAsync();
            })
            .AddCheck<KeycloakCheck>(nameof(KeycloakCheck))
            .AddCheck<TangerineGeneratorServiceCheck>(nameof(TangerineGeneratorServiceCheck));
        services.AddHealthChecksUI(s =>
        {
            s.SetEvaluationTimeInSeconds(300);
        }).AddInMemoryStorage();
    }
    
}