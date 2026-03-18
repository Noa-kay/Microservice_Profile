using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL;

public interface IFileService
{
    Task<IEnumerable<UserFileDto>> GetAllDocumentsAsync(Guid userId, CancellationToken ct = default);
    Task SaveFileAsync(UserFileDto file, CancellationToken ct = default);
    Task DeleteDocumentAsync(Guid documentId, CancellationToken ct = default);
    Task UpdateDocumentDetailsAsync(Guid documentId, UserFileDto newData, CancellationToken ct = default);
}

public class FileService : IFileService
{
    private readonly AppDbContext _context;

    public FileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserFileDto>> GetAllDocumentsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Files.AsNoTracking()
            .Where(f => f.UserId == userId)
            .Select(f => new UserFileDto
            {
                Id = f.Id,
                UserId = f.UserId,
                FileName = f.FileName,
                FileType = f.FileType,
                UploadDate = f.UploadDate,
                FilePath = f.FilePath
            })
            .ToListAsync(ct);
    }

    public async Task SaveFileAsync(UserFileDto file, CancellationToken ct = default)
    {
        var entity = new UserFile
        {
            Id = file.Id == Guid.Empty ? Guid.NewGuid() : file.Id,
            UserId = file.UserId,
            FileName = file.FileName,
            FileType = file.FileType,
            UploadDate = file.UploadDate,
            FilePath = file.FilePath
        };

        _context.Files.Add(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteDocumentAsync(Guid documentId, CancellationToken ct = default)
    {
        var entity = await _context.Files.FirstOrDefaultAsync(f => f.Id == documentId, ct);
        if (entity is null)
        {
            return;
        }

        _context.Files.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateDocumentDetailsAsync(Guid documentId, UserFileDto newData, CancellationToken ct = default)
    {
        var entity = await _context.Files.FirstOrDefaultAsync(f => f.Id == documentId, ct);
        if (entity is null)
        {
            throw new InvalidOperationException("File not found.");
        }

        entity.FileName = newData.FileName;
        entity.FileType = newData.FileType;
        entity.UploadDate = newData.UploadDate;
        entity.FilePath = newData.FilePath;

        await _context.SaveChangesAsync(ct);
    }
}

