using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context) => _context = context;

        public async Task<Project?> GetProjectByIdAsync(Guid projectId, CancellationToken ct = default) =>
            await _context.Projects
                .Include(p => p.User)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == projectId, ct);

        public async Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken ct = default) =>
            await _context.Projects
                .Include(p => p.User)
                .Include(p => p.Images)
                .ToListAsync(ct);

        public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken ct = default) =>
            await _context.Projects
                .Include(p => p.Images)
                .Where(p => p.UserId == userId)
                .ToListAsync(ct);

        public async Task<Project?> GetProjectWithImagesAsync(Guid projectId, CancellationToken ct = default) =>
            await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == projectId, ct);

        public async Task<Project> AddProjectAsync(Project project, CancellationToken ct = default)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync(ct);
            return project;
        }

        public async Task<Project> UpdateProjectAsync(Project project, CancellationToken ct = default)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync(ct);
            return project;
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

        public async Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken ct = default) =>
            await _context.Projects.AnyAsync(p => p.Id == projectId, ct);
    }
}