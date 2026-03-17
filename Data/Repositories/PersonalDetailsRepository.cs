using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public class PersonalDetailsRepository : IPersonalDetailsRepository
    {
        private readonly AppDbContext _context;
        public PersonalDetailsRepository(AppDbContext context) => _context = context;

        public async Task<PersonalDetails?> GetProfileDataAsync(Guid userId, CancellationToken ct = default) =>
            await _context.PersonalDetails.FirstOrDefaultAsync(p => p.UserId == userId, ct);

        public async Task<PersonalDetails> UpdateProfileDataAsync(PersonalDetails details, CancellationToken ct = default)
        {
            _context.PersonalDetails.Update(details);
            await _context.SaveChangesAsync(ct);
            return details;
        }

        public async Task<bool> UpdateEmailAsync(Guid userId, string newEmail, CancellationToken ct = default)
        {
            var details = await _context.PersonalDetails.FirstOrDefaultAsync(p => p.UserId == userId, ct);
            if (details == null) return false;
            details.Email = newEmail;
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> IsEmailAvailableAsync(string email, CancellationToken ct = default) =>
            !await _context.PersonalDetails.AnyAsync(p => p.Email == email, ct);
    }
}