using HealthChecks.UI.Client;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;
using TangerineAuction.Core.Services;
using TangerineAuction.Infrastructure.Data.Services;
using TangerineAuction.Server.Authorization;
using TangerineAuction.Server.Authorization.Impl;
using TangerineAuction.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
ConfigureServices();

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"D:\Pics"),
    RequestPath = "/images"
});
app.UseDefaultFiles();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        options.OAuthClientId("public-client");
        options.OAuthAppName("Tangerine Auction API");
        options.OAuthUsePkce();
        options.OAuthScopeSeparator(" ");
    });
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI();
app.MapFallbackToFile("/index.html");

var logger = app.Services.GetRequiredService<ILogger<Program>>();

WriteAppVersion();
await RunMigrations();

app.Run();

void ConfigureServices()
{
    builder.AddLogger();
    
    builder.Services.AddExceptionHandler<TangerineAuction.Server.Middlewares.GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddResponseCompression(opt => opt.EnableForHttps = true);
    builder.Services.AddSwaggerGenWithAuth(configuration);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    
    var installers = new List<TangerineAuction.Core.Modules.IModuleInstaller>
    {
        new TangerineAuction.Core.Modules.ModuleInstaller(),
        new TangerineAuction.Infrastructure.ModuleInstaller()
    };

    foreach (var installer in installers.OrderBy(x => x.Order))
        installer.Install(builder.Services, configuration);

    builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
    builder.Services.AddAuthorization(options =>
        {
            var admin = "Admin";
            options.AddPolicy(Policy.AddTangerinePolicy, policy => policy.RequireRealmRoles(admin));
            options.AddPolicy(Policy.GetTangerineGeneratorServiceVersion, policy => policy.RequireRealmRoles(admin));
        })
        .AddKeycloakAuthorization(configuration);
    
    // jaeger
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService("TangerineAuction.Server"))
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            tracing.AddOtlpExporter();
        });
    
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddHealthChecks(configuration);
}

void WriteAppVersion()
{
    logger.LogInformation($"App version: {Assembly.GetExecutingAssembly().GetName().Version}");
}

async Task RunMigrations()
{
    using IServiceScope scope = app.Services.CreateScope();
    var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();
    await migrationService.Migrate();
}