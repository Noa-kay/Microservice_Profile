using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default);
        Task<User> CreateAsync(User user, CancellationToken ct = default);
    }
}