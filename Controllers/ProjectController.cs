using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using student_profile.BLL.Interfaces;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // GET: /api/project/user/{userId}
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetProjectsByUserId(Guid userId)
    {
        try
        {
            var projects = await _projectService.GetProjectsByUserIdAsync(userId);
            return Ok(projects);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching projects.");
        }
    }

    // POST: /api/project/user/{userId}
    [HttpPost("user/{userId:guid}")]
    public async Task<IActionResult> AddProject(Guid userId, [FromForm] ProjectDto projectDto, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var created = await _projectService.AddProjectAsync(userId, projectDto, imageFile);
            if (created is null)
            {
                return NotFound("User not found");
            }

            return CreatedAtAction(nameof(GetProjectsByUserId), new { userId }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the project.");
        }
    }

    // DELETE: /api/project/{projectId}
    [HttpDelete("{projectId:guid}")]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        try
        {
            var deleted = await _projectService.DeleteProjectAsync(projectId);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the project.");
        }
    }
}

