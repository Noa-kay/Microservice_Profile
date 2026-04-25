using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using student_profile.BLL;
using student_profile.Common.Security;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Get all projects.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        var projects = await _projectService.GetAllProjectsAsync(cancellationToken);
        return Ok(projects);
    }

    /// <summary>
    /// Get project by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetProjectByIdAsync(id, cancellationToken);
        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    /// <summary>
    /// Get project with images.
    /// </summary>
    [HttpGet("{id:guid}/with-images")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetWithImages(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetProjectWithImagesAsync(id, cancellationToken);
        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    /// <summary>
    /// Get projects by user id.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        if (!CanAccessUserProjects(userId))
        {
            return Forbid();
        }

        var projects = await _projectService.GetProjectsByUserIdAsync(userId, cancellationToken);
        return Ok(projects);
    }

    /// <summary>
    /// Create a new project.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProjectDto>> Create(ProjectDto project, CancellationToken cancellationToken)
    {
        if (!CanAccessUserProjects(project.UserId))
        {
            return Forbid();
        }

        var created = await _projectService.AddProjectAsync(project, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing project.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProjectDto>> Update(Guid id, ProjectDto project, CancellationToken cancellationToken)
    {
        var exists = await _projectService.ProjectExistsAsync(id, cancellationToken);
        if (!exists)
        {
            return NotFound();
        }

        var existingProject = await _projectService.GetProjectByIdAsync(id, cancellationToken);
        if (existingProject is null)
        {
            return NotFound();
        }

        if (!CanAccessUserProjects(existingProject.UserId))
        {
            return Forbid();
        }

        if (!User.IsInAnyRole("Admin") && project.UserId != existingProject.UserId)
        {
            return Forbid();
        }

        project.Id = id;
        var updated = await _projectService.UpdateProjectAsync(project, cancellationToken);
        return Ok(updated);
    }

    /// <summary>
    /// Delete a project.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var existingProject = await _projectService.GetProjectByIdAsync(id, cancellationToken);
        if (existingProject is null)
        {
            return NotFound();
        }

        if (!CanAccessUserProjects(existingProject.UserId))
        {
            return Forbid();
        }

        await _projectService.DeleteProjectAsync(id, cancellationToken);
        return NoContent();
    }

    private bool CanAccessUserProjects(Guid ownerUserId)
    {
        if (User.IsInAnyRole("Admin"))
        {
            return true;
        }

        var tokenUserId = User.GetUserId();
        return tokenUserId.HasValue && tokenUserId.Value == ownerUserId;
    }
}

