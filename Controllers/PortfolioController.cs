using Microsoft.AspNetCore.Mvc;
using student_profile.BLL;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioService _portfolioService;

    public PortfolioController(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    /// <summary>
    /// Get portfolio projects for a user.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        var projects = await _portfolioService.GetProjectsByUserIdAsync(userId, cancellationToken);
        return Ok(projects);
    }

    /// <summary>
    /// Add a project to a user's portfolio.
    /// </summary>
    [HttpPost("user/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddProject(Guid userId, ProjectDto project, CancellationToken cancellationToken)
    {
        await _portfolioService.AddProjectAsync(userId, project, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Update a portfolio project.
    /// </summary>
    [HttpPut("{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateProject(Guid projectId, ProjectDto project, CancellationToken cancellationToken)
    {
        await _portfolioService.UpdateProjectAsync(projectId, project, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete a project from portfolio.
    /// </summary>
    [HttpDelete("{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProject(Guid projectId, CancellationToken cancellationToken)
    {
        await _portfolioService.DeleteProjectAsync(projectId, cancellationToken);
        return NoContent();
    }
}

