using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public interface ISkillRepository
    {
        Task<IEnumerable<Skill>> GetAllSkillsByCategoryAsync(Guid categoryId, CancellationToken ct = default);
        Task<IEnumerable<SkillToUser>> GetUserSkillsAsync(Guid userId, CancellationToken ct = default);
        Task UpdateUserSkillsAsync(Guid userId, List<SkillToUser> skills, CancellationToken ct = default);
    }
}