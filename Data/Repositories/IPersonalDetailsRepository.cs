using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public interface IPersonalDetailsRepository
    {
        Task<PersonalDetails?> GetProfileDataAsync(Guid userId, CancellationToken ct = default);
        Task<PersonalDetails> UpdateProfileDataAsync(PersonalDetails details, CancellationToken ct = default);
        Task<bool> UpdateEmailAsync(Guid userId, string newEmail, CancellationToken ct = default);
        Task<bool> IsEmailAvailableAsync(string email, CancellationToken ct = default);
    }
}