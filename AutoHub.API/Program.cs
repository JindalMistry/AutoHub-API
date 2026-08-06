using Amazon.S3;
using AutoHub.API.Configurations;
using AutoHub.API.Extensions;
using AutoHub.API.HealthChecks;
using AutoHub.API.Middleware;
using AutoHub.Infrastructure.BackgroundJobs;
using AutoHub.Infrastructure.Persistance;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using StackExchange.Redis;
using System.Text.Json;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Serilog Configuration
builder.Services.AddSerilog();
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()

        .WriteTo.Console()

        .WriteTo.File(
            Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            shared: true);
});

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Define the Bearer Auth Scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (e.g., 'Bearer {your_token}')"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbcontext>(options =>
{
    options.UseNpgsql(connectionString);
});

//Hangfire
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString(
                "DefaultConnection"));
    });
});

builder.Services.AddHangfireServer();

//Redis

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect(
        builder.Configuration["Redis:ConnectionString"]!));

// Dependency Injection
builder.Services.AddApplicationServices(builder.Configuration);

//Rate Limiter
builder.Services.AddRateLimiterService();

// Jwt Configuaration
builder.Services.AddJwtAuthentication(
    builder.Configuration);

// AWS
builder.Services.AddAwsServices(builder.Configuration);

// CORS Management

var policyName = "CORSPolicy";

var allowedOrigins = builder.Environment.IsDevelopment()
    ? new[]
    {
        "http://localhost:5173"
    }
    : new[]
    {
        "https://autohub-app-theta.vercel.app"
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy(policyName, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Health Checks

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgres")
    .AddRedis(
        builder.Configuration["Redis:ConnectionString"]!,
        name: "redis")
    .AddCheck<StorageHealthCheck>("S3");


// BUILDER END

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(); // Access at /swagger/index.html

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
    ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseCors(policyName);

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseHttpMetrics();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration,
            Checks = report.Entries.Select(entry => new
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString(),
                Description = entry.Value.Description,
                Duration = entry.Value.Duration
            })
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
    }
});

app.MapMetrics();

var hangfireSettings = builder.Configuration
    .GetSection("Hangfire")
    .Get<HangfireSettings>();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization =
    [
        new BasicAuthAuthorizationFilter(
            new BasicAuthAuthorizationFilterOptions
            {
                RequireSsl = false,
                SslRedirect = false,
                LoginCaseSensitive = true,
                Users =
                [
                    new BasicAuthAuthorizationUser
                    {
                        Login = hangfireSettings!.Username,
                        PasswordClear = hangfireSettings.Password
                    }
                ]
            })
    ]
});

HangfireJobRegistrar.Register();

app.Run();
