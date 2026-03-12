using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetProfileDataAsync(int userId, CancellationToken ct = default);
    Task<User> UpdateProfileDataAsync(User user, CancellationToken ct = default);
    Task<bool> UpdateEmailAsync(int userId, string newEmail, CancellationToken ct = default);
    Task<bool> IsEmailAvailableAsync(string email, CancellationToken ct = default);
}