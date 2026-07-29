using Amazon.S3;
using Amazon.S3.Model;
using AutoHub.Application.Interfaces;
using AutoHub.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.Services;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _amazonS3;
    private readonly StorageSettings _settings;
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(
        IAmazonS3 amazonS3, 
        IOptions<StorageSettings> settings,
        ILogger<S3StorageService> logger
        )
    {
        _amazonS3 = amazonS3;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task DeleteFileAsync(string objectName)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = objectName
        };

        await _amazonS3.DeleteObjectAsync(request);
    }

    public Task<string> GetPresignedUrlAsync(string objectName, TimeSpan expiry)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _settings.BucketName,
            Key = objectName,
            Expires = DateTime.UtcNow.Add(expiry)
        };

        var url = _amazonS3.GetPreSignedURL(request);

        return Task.FromResult(url);
    }

    public async Task<bool> IsHealthyAsync(string bucketName, CancellationToken cancellationToken)
    {
        try
        {
            await _amazonS3.GetBucketLocationAsync(
                new GetBucketLocationRequest
                {
                    BucketName = bucketName
                },
                cancellationToken);

            return true;
        }
        catch(AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 health check failed.");
            return false;
        }
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, string folderName)
    {
        var objectName = $"{folderName}/{Guid.NewGuid()}_{fileName}";

        var request = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = objectName,
            InputStream = stream,
            ContentType = contentType
        };

        await _amazonS3.PutObjectAsync(request);

        return objectName;
    }
}
