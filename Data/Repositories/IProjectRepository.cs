using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public interface IProjectRepository
    {
        Task<Project?> GetProjectByIdAsync(Guid projectId, CancellationToken ct = default);
        Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken ct = default);
        Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<Project?> GetProjectWithImagesAsync(Guid projectId, CancellationToken ct = default);
        Task<Project> AddProjectAsync(Project project, CancellationToken ct = default);
        Task<Project> UpdateProjectAsync(Project project, CancellationToken ct = default);
        Task DeleteProjectAsync(Guid projectId, CancellationToken ct = default);
        Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken ct = default);
    }
}