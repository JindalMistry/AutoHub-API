using AutoHub.Application.Common;
using AutoHub.Application.DTOs.Storage;
using AutoHub.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoHub.API.Controllers;

[ApiController]
[Route("api/storage")]
public class StorageController: ControllerBase
{
    private readonly IStorageService _storageService;

    public StorageController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
    {
        using var stream = request.File.OpenReadStream();

        var objectName =
            await _storageService.UploadFileAsync(
                stream,
                request.File.FileName,
                request.File.ContentType,
                "test");

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Image has been uploaded!",
            Data = objectName
        });
    }
}
