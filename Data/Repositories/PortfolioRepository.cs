using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly AppDbContext _context;

    public PortfolioRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken cancellationToken = default) =>
        await _context.Projects
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task AddProjectAsync(int userId, Project project, CancellationToken cancellationToken = default)
    {
        project.UserId = userId;
        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProjectAsync(int projectId, Project project, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Projects.FindAsync(new object[] { projectId }, cancellationToken);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(project);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.FindAsync(new object[] { projectId }, cancellationToken);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}