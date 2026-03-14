using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace student_profile.BLL.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty.", nameof(file));
        }

        // גודל מקסימלי 5MB
        const long maxSizeBytes = 5 * 1024 * 1024;
        if (file.Length > maxSizeBytes)
        {
            throw new ArgumentException("File size exceeds 5MB limit.", nameof(file));
        }

        // בדיקת סיומת קובץ לתמונות
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Only image files are allowed (.jpg, .jpeg, .png, .gif, .webp).", nameof(file));
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // URL יחסי שניתן להגיש דרך StaticFiles
        var relativeUrl = $"/uploads/{fileName}";
        return relativeUrl;
    }
}

