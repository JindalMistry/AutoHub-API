using AutoHub.Application.Interfaces;
using AutoHub.Infrastructure.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace AutoHub.API.HealthChecks;

public class MinioHealthCheck : IHealthCheck
{
    private readonly IStorageService _storageService;
    private readonly MinioSettings minioSettings;

    public MinioHealthCheck(
        IStorageService storageService,
        IOptions<MinioSettings> _settings)
    {
        _storageService = storageService;
        minioSettings = _settings.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketName = minioSettings.BucketName;

            var exists = await _storageService.IsHealthyAsync(bucketName, cancellationToken);

            return exists
                ? HealthCheckResult.Healthy("MinIO is healthy.")
                : HealthCheckResult.Unhealthy("Bucket does not exist.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Unable to connect to MinIO.",
                ex);
        }
    }
}
