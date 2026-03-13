using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public interface IPortfolioRepository
    {
        Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task AddProjectAsync(Guid userId, Project project, CancellationToken ct = default);
        Task UpdateProjectAsync(Guid projectId, Project project, CancellationToken ct = default);
        Task DeleteProjectAsync(Guid projectId, CancellationToken ct = default);
    }
}