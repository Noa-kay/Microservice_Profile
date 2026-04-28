using MarketingNotificationService.Data.Entities;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using student_profile.BLL;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPublishEndpoint _publishEndpoint;

    public UsersController(IUserService userService, IPublishEndpoint publishEndpoint)
    {
        _userService = userService;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    /// Get user by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Create a new user. Returns the created user and total user count; publishes count to message bus.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserCreatedResult), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserCreatedResult>> Create(
        UserDto user,
        CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(user, cancellationToken);
        await _publishEndpoint.Publish(
            new graduates_count(result.TotalUsers, DateTime.UtcNow),
            cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.User.Id }, result);
    }
}
