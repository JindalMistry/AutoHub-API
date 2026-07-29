using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using AutoHub.Infrastructure.Configuration;

namespace AutoHub.API.Extensions;

public static class AWSExtensions
{
    public static IServiceCollection AddAwsServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var storageSettings =
            configuration.GetSection("Storage").Get<StorageSettings>()!;

        var awsOptions = new AWSOptions
        {
            Region = RegionEndpoint.GetBySystemName(storageSettings.Region)
        };

        if (!string.IsNullOrWhiteSpace(storageSettings.AccessKey) &&
            !string.IsNullOrWhiteSpace(storageSettings.SecretKey))
        {
            awsOptions.Credentials = new BasicAWSCredentials(
                storageSettings.AccessKey,
                storageSettings.SecretKey);
        }

        services.AddDefaultAWSOptions(awsOptions);

        services.AddAWSService<IAmazonS3>();

        return services;
    }
}