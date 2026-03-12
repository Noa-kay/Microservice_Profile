using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetProjectByIdAsync(int projectId, CancellationToken ct = default);
    Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken ct = default);
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Project?> GetProjectWithImagesAsync(int projectId, CancellationToken ct = default);
    Task<Project> AddProjectAsync(Project project, CancellationToken ct = default);
    Task<Project> UpdateProjectAsync(Project project, CancellationToken ct = default);
    Task DeleteProjectAsync(int projectId, CancellationToken ct = default);
    Task<bool> ProjectExistsAsync(int projectId, CancellationToken ct = default);
}