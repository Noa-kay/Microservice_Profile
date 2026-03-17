using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL;

public interface IImageService
{
    Task<ImageDto?> GetImageByIdAsync(Guid imageId, CancellationToken ct = default);
    Task<IEnumerable<ImageDto>> GetAllImagesAsync(CancellationToken ct = default);
    Task<IEnumerable<ImageDto>> GetImagesByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<ImageDto> AddImageAsync(ImageDto image, CancellationToken ct = default);
    Task<ImageDto> UpdateImageAsync(ImageDto image, CancellationToken ct = default);
    Task DeleteImageAsync(Guid imageId, CancellationToken ct = default);
    Task<bool> ImageExistsAsync(Guid imageId, CancellationToken ct = default);
}

public class ImageService : IImageService
{
    private readonly AppDbContext _context;

    public ImageService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ImageDto?> GetImageByIdAsync(Guid imageId, CancellationToken ct = default)
    {
        var entity = await _context.Images.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == imageId, ct);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<IEnumerable<ImageDto>> GetAllImagesAsync(CancellationToken ct = default)
    {
        var entities = await _context.Images.AsNoTracking().ToListAsync(ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ImageDto>> GetImagesByProjectIdAsync(Guid projectId, CancellationToken ct = default)
    {
        var entities = await _context.Images.AsNoTracking()
            .Where(i => i.ProjectId == projectId)
            .ToListAsync(ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<ImageDto> AddImageAsync(ImageDto image, CancellationToken ct = default)
    {
        var entity = MapToEntity(image);
        entity.Id = Guid.NewGuid();
        _context.Images.Add(entity);
        await _context.SaveChangesAsync(ct);
        image.Id = entity.Id;
        return image;
    }

    public async Task<ImageDto> UpdateImageAsync(ImageDto image, CancellationToken ct = default)
    {
        var entity = await _context.Images.FirstOrDefaultAsync(i => i.Id == image.Id, ct);
        if (entity is null)
        {
            throw new InvalidOperationException("Image not found.");
        }

        entity.ProjectId = image.ProjectId;
        entity.ImageName = image.ImageName;
        entity.URL = image.URL;

        await _context.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task DeleteImageAsync(Guid imageId, CancellationToken ct = default)
    {
        var entity = await _context.Images.FirstOrDefaultAsync(i => i.Id == imageId, ct);
        if (entity is null)
        {
            return;
        }

        _context.Images.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ImageExistsAsync(Guid imageId, CancellationToken ct = default)
    {
        return await _context.Images.AnyAsync(i => i.Id == imageId, ct);
    }

    private static ImageDto MapToDto(Image entity) =>
        new()
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            ImageName = entity.ImageName,
            URL = entity.URL
        };

    private static Image MapToEntity(ImageDto dto) =>
        new()
        {
            Id = dto.Id,
            ProjectId = dto.ProjectId,
            ImageName = dto.ImageName,
            URL = dto.URL
        };
}

