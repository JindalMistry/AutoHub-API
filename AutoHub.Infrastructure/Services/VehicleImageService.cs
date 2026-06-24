using AutoHub.Application.DTOs.Vehicles;
using AutoHub.Application.Exceptions;
using AutoHub.Application.Interfaces;
using AutoHub.Domain.Entities;
using AutoHub.Domain.Enums;
using AutoHub.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace AutoHub.Infrastructure.Services;

public class VehicleImageService : IVehicleImageService
{
    private readonly ApplicationDbcontext _dbcontext;
    
    private readonly IStorageService _storageService;

    public VehicleImageService(ApplicationDbcontext dbcontext, IStorageService storageService)
    {
        _dbcontext = dbcontext;
        _storageService = storageService;
    }

    public async Task DeleteImageAsync(Guid imageId, Guid userId)
    {
        var dealer = await _dbcontext.Dealers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == userId);

        if (dealer == null) throw new NotFoundException("Dealer does not exist!");

        if (dealer.Status != DealerStatus.Approved)
            throw new ForbiddenException("Dealer is not approved!");

        var image = await _dbcontext.VehicleImages
            .FirstOrDefaultAsync(o => o.Id == imageId);

        if (image == null) throw new NotFoundException("Image does not exist!");

        var vehicle = await _dbcontext.Vehicles
            .FirstOrDefaultAsync(o => o.Id == image.VehicleId && o.DealerId == dealer.Id);

        if (vehicle == null) throw new ForbiddenException("Vehicle does not belong to dealer!");

        if (vehicle.Status != VehicleStatus.Draft) throw new ForbiddenException("Images can only be deleted from draft vehicles.");

        try
        {
            await _storageService.DeleteFileAsync(image.ImageUrl);
        }
        catch
        {
            throw new InternalServerException("Failed to delete the image from bucket!");
        }

        _dbcontext.VehicleImages.Remove(image);

        await _dbcontext.SaveChangesAsync();
    }

    public async Task<List<VehicleImageResponse>> GetImagesAsync(Guid vehicleId)
    {
        return await _dbcontext.VehicleImages
            .AsNoTracking()
            .Where(o => o.VehicleId == vehicleId)
            .OrderBy(o => o.DisplayOrder)
            .Select(o => new VehicleImageResponse
            {
                Id = o.Id,
                ImageUrl = o.ImageUrl,
                DisplayOrder = o.DisplayOrder,
            })
            .ToListAsync();
    }

    public async Task<List<VehicleImageResponse>> UploadImagesAsync(CreateVehicleImageRequest request)
    {
        var dealer = await _dbcontext.Dealers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == request.UserId);

        if (dealer == null) throw new NotFoundException("Dealer does not exist!");

        if (dealer.Status != DealerStatus.Approved)
            throw new ForbiddenException("Dealer is not approved!");

        var vehicle = await _dbcontext.Vehicles
            .FirstOrDefaultAsync(o => o.Id == request.VehicleId && o.DealerId == dealer.Id);

        if (vehicle == null) throw new NotFoundException("Vehicle does not exist!");

        if (vehicle.Status != VehicleStatus.Draft)
            throw new ForbiddenException("Images can only be uploaded to draft vehicles.");

        if (!request.Files.Any())
        {
            throw new BadRequestException(
                "At least one image is required.");
        }

        if (request.Files.Count > 20)
        {
            throw new BadRequestException(
                "Maximum 20 images per upload request.");
        }

        var MaxFileSize = 10 * 1024 * 1024;

        if (request.Files.Any(f => f.Length > MaxFileSize))
        {
            throw new BadRequestException(
                "One or more images exceed 10 MB.");
        }

        var allowedExtensions = new[]{ ".jpg", ".jpeg", ".png", ".webp" };

        if (request.Files.Any(f =>
            !allowedExtensions.Contains(
                Path.GetExtension(f.FileName)
                    .ToLower())))
        {
            throw new BadRequestException(
                "Only jpg, jpeg, png and webp files are allowed.");
        }

        var displayOrder =
            await _dbcontext.VehicleImages
                .Where(o => o.VehicleId == request.VehicleId)
                .MaxAsync(o => (int?)o.DisplayOrder) ?? 0;

        var uploadedImages =
            new List<VehicleImageResponse>();

        foreach (var file in request.Files)
        {
            displayOrder++;

            await using var stream = file.OpenReadStream();

            var objectKey = await _storageService.UploadFileAsync(
                stream,
                file.FileName,
                file.ContentType,
                "vehicles");

            var vehicleImage = new VehicleImage
            {
                Id = Guid.NewGuid(),
                VehicleId = request.VehicleId,
                ImageUrl = objectKey,
                DisplayOrder = displayOrder,
                CreatedAt = DateTime.UtcNow
            };

            _dbcontext.VehicleImages.Add(vehicleImage);

            uploadedImages.Add(new VehicleImageResponse
            {
                Id = vehicleImage.Id,
                ImageUrl = vehicleImage.ImageUrl,
                DisplayOrder = vehicleImage.DisplayOrder
            });
        }

        await _dbcontext.SaveChangesAsync();

        return uploadedImages;
    }
}