using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public class ResumeRepository : IResumeRepository
{
    private readonly AppDbContext _context;

    public ResumeRepository(AppDbContext context) => _context = context;

    public async Task<Resume?> GetByIdAsync(int resumeId, CancellationToken ct = default) =>
        await _context.Resumes
            .Include(r => r.User)
            .Include(r => r.ExportedFile)
            .FirstOrDefaultAsync(r => r.Id == resumeId, ct);

    public async Task<IEnumerable<Resume>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Resumes
            .Include(r => r.ExportedFile)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<Resume?> GetActiveResumeAsync(Guid userId, string targetRole, CancellationToken ct = default) =>
        await _context.Resumes
            .Include(r => r.ExportedFile)
            .FirstOrDefaultAsync(r =>
                r.UserId == userId &&
                r.TargetRole == targetRole &&
                r.IsActive, ct);

    public async Task<Resume> CreateAsync(Resume resume, CancellationToken ct = default)
    {
        resume.CreatedAt = DateTime.UtcNow;
        _context.Resumes.Add(resume);
        await _context.SaveChangesAsync(ct);
        return resume;
    }

    public async Task<Resume> UpdateAsync(Resume resume, CancellationToken ct = default)
    {
        resume.LastModified = DateTime.UtcNow;
        _context.Resumes.Update(resume);
        await _context.SaveChangesAsync(ct);
        return resume;
    }

    public async Task DeleteAsync(int resumeId, CancellationToken ct = default)
    {
        var resume = await _context.Resumes.FindAsync(new object[] { resumeId }, ct);
        if (resume != null)
        {
            _context.Resumes.Remove(resume);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> SetActiveAsync(int resumeId, Guid userId, CancellationToken ct = default)
    {
        var resume = await _context.Resumes.FindAsync(new object[] { resumeId }, ct);
        if (resume == null || resume.UserId != userId) return false;

        // Deactivate all other resumes for this user with same target role
        var otherResumes = await _context.Resumes
            .Where(r => r.UserId == userId &&
                       r.TargetRole == resume.TargetRole &&
                       r.Id != resumeId)
            .ToListAsync(ct);

        foreach (var other in otherResumes)
        {
            other.IsActive = false;
        }

        resume.IsActive = true;
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
