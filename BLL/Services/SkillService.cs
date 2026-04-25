using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL;

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetAllSkillsByCategoryAsync(Guid categoryId, CancellationToken ct = default);
    Task<IEnumerable<SkillToUserDto>> GetUserSkillsAsync(Guid userId, CancellationToken ct = default);
    Task UpdateUserSkillsAsync(Guid userId, List<SkillToUserDto> skills, CancellationToken ct = default);
}

public class SkillService : ISkillService
{
    private readonly AppDbContext _context;

    public SkillService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SkillDto>> GetAllSkillsByCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await _context.Skills.AsNoTracking()
            .Where(s => s.CategoryId == categoryId)
            .Select(s => new SkillDto
            {
                Id = s.Id,
                Name = s.Name,
                CategoryId = s.CategoryId
            })
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<SkillToUserDto>> GetUserSkillsAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.SkillToUsers.AsNoTracking()
            .Where(su => su.UserId == userId)
            .Select(su => new SkillToUserDto
            {
                UserId = su.UserId,
                SkillId = su.SkillId,
                YearsOfExperience = su.YearsOfExperience
            })
            .ToListAsync(ct);
    }

    public async Task UpdateUserSkillsAsync(Guid userId, List<SkillToUserDto> skills, CancellationToken ct = default)
    {
        var existing = await _context.SkillToUsers
            .Where(su => su.UserId == userId)
            .ToListAsync(ct);

        _context.SkillToUsers.RemoveRange(existing);

        var entities = skills.Select(s => new SkillToUser
        {
            UserId = userId,
            SkillId = s.SkillId,
            YearsOfExperience = s.YearsOfExperience
        });

        await _context.SkillToUsers.AddRangeAsync(entities, ct);
        await _context.SaveChangesAsync(ct);
    }
}

