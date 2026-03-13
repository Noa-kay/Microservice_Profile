using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _context;
        public ImageRepository(AppDbContext context) => _context = context;

        public async Task<Image?> GetImageByIdAsync(Guid imageId, CancellationToken ct = default) =>
            await _context.Images
                .Include(i => i.Project)
                .FirstOrDefaultAsync(i => i.Id == imageId, ct);

        public async Task<IEnumerable<Image>> GetAllImagesAsync(CancellationToken ct = default) =>
            await _context.Images
                .Include(i => i.Project)
                .ToListAsync(ct);

        public async Task<IEnumerable<Image>> GetImagesByProjectIdAsync(Guid projectId, CancellationToken ct = default) =>
            await _context.Images
                .Where(i => i.ProjectId == projectId)
                .ToListAsync(ct);

        public async Task<Image> AddImageAsync(Image image, CancellationToken ct = default)
        {
            _context.Images.Add(image);
            await _context.SaveChangesAsync(ct);
            return image;
        }

        public async Task<Image> UpdateImageAsync(Image image, CancellationToken ct = default)
        {
            _context.Images.Update(image);
            await _context.SaveChangesAsync(ct);
            return image;
        }

        public async Task DeleteImageAsync(Guid imageId, CancellationToken ct = default)
        {
            var image = await _context.Images.FindAsync(new object[] { imageId }, ct);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<bool> ImageExistsAsync(Guid imageId, CancellationToken ct = default) =>
            await _context.Images.AnyAsync(i => i.Id == imageId, ct);
    }
}