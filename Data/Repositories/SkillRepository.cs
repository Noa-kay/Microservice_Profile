using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly AppDbContext _context;
    public SkillRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Skill>> GetAllSkillsByCategoryAsync(int categoryId, CancellationToken ct = default) =>
        await _context.Skills
            .Where(s => s.CategoryId == categoryId)
            .ToListAsync(ct);

    public async Task<IEnumerable<SkillToUser>> GetUserSkillsAsync(int userId, CancellationToken ct = default) =>
        await _context.SkillToUsers
            .Include(stu => stu.Skill)
            .Where(stu => stu.UserId == userId)
            .ToListAsync(ct);

    // *** FIXED: Now properly handles YearsOfExperience ***
    public async Task UpdateUserSkillsAsync(int userId, List<UserSkillUpdateDto> skills, CancellationToken ct = default)
    {
        // Remove existing skills for this user
        var existing = _context.SkillToUsers.Where(stu => stu.UserId == userId);
        _context.SkillToUsers.RemoveRange(existing);

        // Add new skills with their YearsOfExperience
        var newSkills = skills.Select(s => new SkillToUser 
        { 
            UserId = userId, 
            SkillId = s.SkillId,
            YearsOfExperience = s.YearsOfExperience 
        });
        
        await _context.SkillToUsers.AddRangeAsync(newSkills, ct);
        await _context.SaveChangesAsync(ct);
    }
}