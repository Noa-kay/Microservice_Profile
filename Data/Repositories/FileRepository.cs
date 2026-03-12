using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public class FileRepository : IFileRepository
{
    private readonly AppDbContext _context;
    public FileRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<student_profile.Data.Models.UserFile>> GetAllDocumentsAsync(int userId, CancellationToken ct = default) =>
        await _context.Files.Where(f => f.UserId == userId).ToListAsync(ct);

    public async Task SaveFileAsync(student_profile.Data.Models.UserFile file, CancellationToken ct = default)
    {
        _context.Files.Add(file);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteDocumentAsync(int documentId, CancellationToken ct = default)
    {
        var file = await _context.Files.FindAsync(new object[] { documentId }, ct);
        if (file != null)
        {
            _context.Files.Remove(file);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task UpdateDocumentDetailsAsync(int documentId, student_profile.Data.Models.UserFile newData, CancellationToken ct = default)
    {
        var existing = await _context.Files.FindAsync(new object[] { documentId }, ct);
        if (existing != null)
        {
            existing.FileName = newData.FileName;
            existing.FileType = newData.FileType;
            await _context.SaveChangesAsync(ct);
        }
    }
}