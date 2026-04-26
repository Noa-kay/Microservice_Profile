using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using student_profile.BLL;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IFileService _fileService;
    private readonly IUserService _userService;

    public ProjectController(
        IProjectService projectService,
        IFileService fileService,
        IUserService userService)
    {
        _projectService = projectService;
        _fileService = fileService;
        _userService = userService;
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
            var user = await _userService.GetByIdAsync(userId);
            if (user is null)
            {
                return NotFound("User not found");
            }

            projectDto.UserId = userId;
            if (imageFile is not null && imageFile.Length > 0)
            {
                projectDto.ProjectsImages = await _fileService.UploadFileAsync(imageFile);
            }

            var created = await _projectService.AddProjectAsync(projectDto);
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
            if (!await _projectService.ProjectExistsAsync(projectId))
            {
                return NotFound();
            }

            await _projectService.DeleteProjectAsync(projectId);
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
