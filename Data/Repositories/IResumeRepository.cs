using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public interface IResumeRepository
{
    Task<Resume?> GetByIdAsync(int resumeId, CancellationToken ct = default);
    Task<IEnumerable<Resume>> GetAllByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<Resume?> GetActiveResumeAsync(Guid userId, string targetRole, CancellationToken ct = default);
    Task<Resume> CreateAsync(Resume resume, CancellationToken ct = default);
    Task<Resume> UpdateAsync(Resume resume, CancellationToken ct = default);
    Task DeleteAsync(int resumeId, CancellationToken ct = default);
    Task<bool> SetActiveAsync(int resumeId, Guid userId, CancellationToken ct = default);
}
