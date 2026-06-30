namespace AutoHub.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(
        Stream stream,
        string fileName,
        string contentType,
        string folderName);

    Task DeleteFileAsync(string objectName);

    Task<bool> IsHealthyAsync(string BucketName, CancellationToken cancellationToken);
}
