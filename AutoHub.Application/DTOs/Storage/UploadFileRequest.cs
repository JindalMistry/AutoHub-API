using Microsoft.AspNetCore.Http;

namespace AutoHub.Application.DTOs.Storage;

public class UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
}
