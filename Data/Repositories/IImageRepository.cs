using student_profile.Data.Models;
 
namespace student_profile.Data.Repositories;
 
public interface IImageRepository
{
    Task<Image?> GetImageByIdAsync(int imageId, CancellationToken ct = default);
    Task<IEnumerable<Image>> GetAllImagesAsync(CancellationToken ct = default);
    Task<IEnumerable<Image>> GetImagesByProjectIdAsync(int projectId, CancellationToken ct = default);
    Task<Image> AddImageAsync(Image image, CancellationToken ct = default);
    Task<Image> UpdateImageAsync(Image image, CancellationToken ct = default);
    Task DeleteImageAsync(int imageId, CancellationToken ct = default);
    Task<bool> ImageExistsAsync(int imageId, CancellationToken ct = default);
}
 