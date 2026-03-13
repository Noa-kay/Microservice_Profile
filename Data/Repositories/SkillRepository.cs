using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly AppDbContext _context;
        public SkillRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Skill>> GetAllSkillsByCategoryAsync(Guid categoryId, CancellationToken ct = default) =>
            await _context.Skills
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync(ct);

        public async Task<IEnumerable<SkillToUser>> GetUserSkillsAsync(Guid userId, CancellationToken ct = default) =>
            await _context.SkillToUsers
                .Include(stu => stu.Skill)
                .Where(stu => stu.UserId == userId)
                .ToListAsync(ct);

        // עדכון משתמשים בלי שימוש ב-UserSkillUpdateDto
        public async Task UpdateUserSkillsAsync(Guid userId, List<SkillToUser> skills, CancellationToken ct = default)
        {
            var existing = _context.SkillToUsers.Where(stu => stu.UserId == userId);
            _context.SkillToUsers.RemoveRange(existing);

            // הוספה של רשימת SkillToUser ישירות
            await _context.SkillToUsers.AddRangeAsync(skills, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}