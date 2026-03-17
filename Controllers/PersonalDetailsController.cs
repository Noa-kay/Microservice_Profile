using Microsoft.AspNetCore.Mvc;
using student_profile.BLL;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonalDetailsController : ControllerBase
{
    private readonly IPersonalDetailsService _personalDetailsService;

    public PersonalDetailsController(IPersonalDetailsService personalDetailsService)
    {
        _personalDetailsService = personalDetailsService;
    }

    /// <summary>
    /// Get profile data for a given user.
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(PersonalDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonalDetailsDto>> Get(Guid userId, CancellationToken cancellationToken)
    {
        var details = await _personalDetailsService.GetProfileDataAsync(userId, cancellationToken);
        if (details is null)
        {
            return NotFound();
        }

        return Ok(details);
    }

    /// <summary>
    /// Update profile data for a user.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(PersonalDetailsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PersonalDetailsDto>> Update(PersonalDetailsDto details, CancellationToken cancellationToken)
    {
        var updated = await _personalDetailsService.UpdateProfileDataAsync(details, cancellationToken);
        return Ok(updated);
    }

    /// <summary>
    /// Update user email.
    /// </summary>
    [HttpPatch("{userId:guid}/email")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEmail(Guid userId, [FromBody] string newEmail, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
        {
            return BadRequest("Email is required.");
        }

        var isAvailable = await _personalDetailsService.IsEmailAvailableAsync(newEmail, cancellationToken);
        if (!isAvailable)
        {
            return BadRequest("Email is already in use.");
        }

        var updated = await _personalDetailsService.UpdateEmailAsync(userId, newEmail, cancellationToken);
        if (!updated)
        {
            return BadRequest("Failed to update email.");
        }

        return NoContent();
    }
}

