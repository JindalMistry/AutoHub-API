namespace AutoHub.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(
        Stream stream,
        string fileName,
        string contentType,
        string folderName);

    Task DeleteFileAsync(string objectName);

    Task<string> GetPresignedUrlAsync(
        string objectName,
        TimeSpan expiry);

    Task<bool> IsHealthyAsync(string bucketName, CancellationToken cancellationToken);
}
