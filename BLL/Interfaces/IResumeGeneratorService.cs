using student_profile.Data.Models;

namespace student_profile.BLL.Interfaces;

public interface IResumeGeneratorService
{
    Task<Resume> GenerateResumeAsync(
        Guid userId,
        string title,
        string targetRole,
        ResumeTemplate template,
        string language,
        string tone,
        bool setAsActive,
        CancellationToken ct = default);
}
