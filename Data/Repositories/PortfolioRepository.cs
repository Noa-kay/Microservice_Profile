using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDbContext _context;
        public PortfolioRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken ct = default) =>
            await _context.Projects
                .Where(p => p.UserId == userId)
                .ToListAsync(ct);

        public async Task AddProjectAsync(Guid userId, Project project, CancellationToken ct = default)
        {
            project.UserId = userId;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateProjectAsync(Guid projectId, Project project, CancellationToken ct = default)
        {
            var existing = await _context.Projects.FindAsync(new object[] { projectId }, ct);
            if (existing != null)
            {
                _context.Entry(existing).CurrentValues.SetValues(project);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task DeleteProjectAsync(Guid projectId, CancellationToken ct = default)
        {
            var project = await _context.Projects.FindAsync(new object[] { projectId }, ct);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}