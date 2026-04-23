using Microsoft.AspNetCore.Http;
using student_profile.DTOs;

namespace student_profile.BLL.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(Guid userId);
    Task<ProjectDto?> AddProjectAsync(Guid userId, ProjectDto projectDto, IFormFile? imageFile);
    Task<bool> DeleteProjectAsync(Guid projectId);
}

