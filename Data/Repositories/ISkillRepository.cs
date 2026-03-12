using student_profile.Data.Models;
 
namespace student_profile.Data.Repositories;
 
// DTO for updating user skills with experience
public class UserSkillUpdateDto
{
    public int SkillId { get; set; }
    public int YearsOfExperience { get; set; }
}
 
public interface ISkillRepository
{
    Task<IEnumerable<Skill>> GetAllSkillsByCategoryAsync(int categoryId, CancellationToken ct = default);
    Task<IEnumerable<SkillToUser>> GetUserSkillsAsync(int userId, CancellationToken ct = default);
    
    // *** FIXED: Changed signature to accept YearsOfExperience ***
    Task UpdateUserSkillsAsync(int userId, List<UserSkillUpdateDto> skills, CancellationToken ct = default);
}
 