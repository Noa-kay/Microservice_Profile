using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL;

public interface IPortfolioService
{
    Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddProjectAsync(Guid userId, ProjectDto project, CancellationToken ct = default);
    Task UpdateProjectAsync(Guid projectId, ProjectDto project, CancellationToken ct = default);
    Task DeleteProjectAsync(Guid projectId, CancellationToken ct = default);
}

public class PortfolioService : IPortfolioService
{
    private readonly AppDbContext _context;

    public PortfolioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _context.Projects.AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                UserId = p.UserId,
                Title = p.Title,
                Description = p.Description,
                ProjectName = p.ProjectName,
                GitHubLink = p.GitHubLink,
                ProjectsImages = p.ProjectsImages
            })
            .ToListAsync(ct);
    }

    public async Task AddProjectAsync(Guid userId, ProjectDto project, CancellationToken ct = default)
    {
        var entity = new Project
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = project.Title,
            Description = project.Description,
            ProjectName = project.ProjectName,
            GitHubLink = project.GitHubLink,
            ProjectsImages = project.ProjectsImages
        };

        _context.Projects.Add(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateProjectAsync(Guid projectId, ProjectDto project, CancellationToken ct = default)
    {
        var entity = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId, ct);
        if (entity is null)
        {
            throw new InvalidOperationException("Project not found.");
        }

        entity.Title = project.Title;
        entity.Description = project.Description;
        entity.ProjectName = project.ProjectName;
        entity.GitHubLink = project.GitHubLink;
        entity.ProjectsImages = project.ProjectsImages;

        await _context.SaveChangesAsync(ct);
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
}

