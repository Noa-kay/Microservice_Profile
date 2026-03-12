using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetProfileDataAsync(int userId, CancellationToken ct = default) =>
        await _context.Users
            .Include(u => u.Projects)
            .Include(u => u.SkillToUsers).ThenInclude(stu => stu.Skill)
            .Include(u => u.Files)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

    public async Task<User> UpdateProfileDataAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> IsEmailAvailableAsync(string email, CancellationToken ct = default) =>
        !await _context.Users.AnyAsync(u => u.Mail == email, ct);

    public async Task<bool> UpdateEmailAsync(int userId, string newEmail, CancellationToken ct = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, ct);
        if (user == null) return false;
        user.Mail = newEmail;
        await _context.SaveChangesAsync(ct);
        return true;
    }
}