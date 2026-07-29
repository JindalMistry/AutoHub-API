using AutoHub.Application.Interfaces;
using AutoHub.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.Services;

public class MinIOStorageService : IStorageService
{
    private readonly IMinioClient _minioClient;

    private readonly MinioSettings _settings;

    public MinIOStorageService(IOptions<MinioSettings> settings)
    {
        _settings = settings.Value;

        _minioClient = new MinioClient()
            .WithEndpoint(_settings.Endpoint)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .Build();
    }

    public async Task DeleteFileAsync(string objectName)
    {
        await _minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectName));
    }

    public Task<string> GetPresignedUrlAsync(string objectName, TimeSpan expiry)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsHealthyAsync(string bucketName, CancellationToken cancellationToken)
    {
        return await _minioClient.BucketExistsAsync(
                new BucketExistsArgs()
                    .WithBucket(bucketName),
                cancellationToken);
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, string contentType, string folderName)
    {
        var objectName =
        $"{folderName}/{Guid.NewGuid()}_{fileName}";

        await _minioClient.PutObjectAsync(
         new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType));

        return objectName;
    }
}
