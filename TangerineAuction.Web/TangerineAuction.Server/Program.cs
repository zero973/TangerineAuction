using Hangfire;
using Hangfire.PostgreSql;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Reflection;
using TangerineAuction.Core.Modules;
using TangerineAuction.Core.Services;
using TangerineAuction.Infrastructure.Data.Services;
using TangerineAuction.Server.Authorization.Impl;
using TangerineAuction.Server.Extensions;
using TangerineAuction.Server.Jobs;
using TangerineAuction.Server.Middlewares;
using TangerineAuction.Server.SignalR;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
ConfigureServices();

var app = builder.Build();

app.UseStaticFiles();
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
app.UseHangfireDashboard();
app.UseCors();

app.MapControllers();
app.MapHangfireDashboard();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI();
app.MapHub<AuctionHub>("/auctionHub");
app.MapFallbackToFile("/index.html");

var logger = app.Services.GetRequiredService<ILogger<Program>>();

WriteAppVersion();
await RunMigrations();
RunRecurringJob();

app.Run();

void ConfigureServices()
{
    builder.AddLogger();
    
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddResponseCompression(opt => opt.EnableForHttps = true);
    builder.Services.AddSwaggerGenWithAuth(configuration);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddHangfire((_, config) => 
        config.UsePostgreSqlStorage(opts => 
            opts.UseNpgsqlConnection(configuration.GetConnectionString("Postgres"))));
    builder.Services.AddHangfireServer();
    builder.Services.AddSignalR();
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:64229")
                .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
    
    var installers = new List<IModuleInstaller>
    {
        new ModuleInstaller(),
        new TangerineAuction.Infrastructure.ModuleInstaller()
    };

    foreach (var installer in installers.OrderBy(x => x.Order))
        installer.Install(builder.Services, configuration);

    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    });
    
    builder.Services.AddScoped<IJobRunner, JobRunner>();
    builder.Services.AddKeycloak(configuration);
    builder.Services.AddJaeger();
    
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

void RunRecurringJob()
{
    using IServiceScope scope = app.Services.CreateScope();
    var jobRunner = scope.ServiceProvider.GetRequiredService<IJobRunner>();
    jobRunner.Run();
}