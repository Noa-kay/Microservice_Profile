using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL;

public interface IProjectService
{
    Task<ProjectDto?> GetProjectByIdAsync(Guid projectId, CancellationToken ct = default);
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(CancellationToken ct = default);
    Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<ProjectDto?> GetProjectWithImagesAsync(Guid projectId, CancellationToken ct = default);
    Task<ProjectDto> AddProjectAsync(ProjectDto project, CancellationToken ct = default);
    Task<ProjectDto> UpdateProjectAsync(ProjectDto project, CancellationToken ct = default);
    Task DeleteProjectAsync(Guid projectId, CancellationToken ct = default);
    Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken ct = default);
}

public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(Guid projectId, CancellationToken ct = default)
    {
        var entity = await _context.Projects.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == projectId, ct);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(CancellationToken ct = default)
    {
        var entities = await _context.Projects.AsNoTracking().ToListAsync(ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var entities = await _context.Projects.AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync(ct);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<ProjectDto?> GetProjectWithImagesAsync(Guid projectId, CancellationToken ct = default)
    {
        var entity = await _context.Projects.AsNoTracking()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == projectId, ct);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<ProjectDto> AddProjectAsync(ProjectDto project, CancellationToken ct = default)
    {
        var entity = MapToEntity(project);
        entity.Id = Guid.NewGuid();
        _context.Projects.Add(entity);
        await _context.SaveChangesAsync(ct);
        project.Id = entity.Id;
        return project;
    }

    public async Task<ProjectDto> UpdateProjectAsync(ProjectDto project, CancellationToken ct = default)
    {
        var entity = await _context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id, ct);
        if (entity is null)
        {
            throw new InvalidOperationException("Project not found.");
        }

        entity.UserId = project.UserId;
        entity.Title = project.Title;
        entity.Description = project.Description;
        entity.ProjectName = project.ProjectName;
        entity.GitHubLink = project.GitHubLink;
        entity.ProjectsImages = project.ProjectsImages;

        await _context.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task DeleteProjectAsync(Guid projectId, CancellationToken ct = default)
    {
        var entity = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId, ct);
        if (entity is null)
        {
            return;
        }

        _context.Projects.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken ct = default)
    {
        return await _context.Projects.AnyAsync(p => p.Id == projectId, ct);
    }

    private static ProjectDto MapToDto(Project entity) =>
        new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Title = entity.Title,
            Description = entity.Description,
            ProjectName = entity.ProjectName,
            GitHubLink = entity.GitHubLink,
            ProjectsImages = entity.ProjectsImages
        };

    private static Project MapToEntity(ProjectDto dto) =>
        new()
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Title = dto.Title,
            Description = dto.Description,
            ProjectName = dto.ProjectName,
            GitHubLink = dto.GitHubLink,
            ProjectsImages = dto.ProjectsImages
        };
}

