using Microsoft.AspNetCore.Mvc;
using student_profile.BLL;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    /// <summary>
    /// Get all skills for a given category.
    /// </summary>
    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<SkillDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetByCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var skills = await _skillService.GetAllSkillsByCategoryAsync(categoryId, cancellationToken);
        return Ok(skills);
    }

    /// <summary>
    /// Get skills assigned to a user.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<SkillToUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SkillToUserDto>>> GetUserSkills(Guid userId, CancellationToken cancellationToken)
    {
        var skills = await _skillService.GetUserSkillsAsync(userId, cancellationToken);
        return Ok(skills);
    }

    /// <summary>
    /// Replace user skills.
    /// </summary>
    [HttpPut("user/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserSkills(Guid userId, [FromBody] List<SkillToUserDto> skills, CancellationToken cancellationToken)
    {
        await _skillService.UpdateUserSkillsAsync(userId, skills, cancellationToken);
        return NoContent();
    }
}

