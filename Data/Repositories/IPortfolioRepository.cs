using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public interface IPortfolioRepository
{
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken ct = default);
    Task AddProjectAsync(int userId, Project project, CancellationToken ct = default);
    Task UpdateProjectAsync(int projectId, Project project, CancellationToken ct = default);
    Task DeleteProjectAsync(int projectId, CancellationToken ct = default);
}