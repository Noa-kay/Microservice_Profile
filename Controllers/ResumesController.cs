using Microsoft.AspNetCore.Mvc;
using student_profile.BLL.Interfaces;
using student_profile.Data.Models;
using student_profile.Data.Repositories;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumesController : ControllerBase
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IResumeGeneratorService _resumeGeneratorService;

    public ResumesController(
        IResumeRepository resumeRepository,
        IResumeGeneratorService resumeGeneratorService)
    {
        _resumeRepository = resumeRepository;
        _resumeGeneratorService = resumeGeneratorService;
    }

    [HttpPost("generate")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResumeDto>> Generate(
        [FromBody] GenerateResumeRequestDto request,
        CancellationToken ct)
    {
        if (request.UserId == Guid.Empty || string.IsNullOrWhiteSpace(request.TargetRole))
        {
            return BadRequest("UserId and TargetRole are required.");
        }

        try
        {
            var resume = await _resumeGeneratorService.GenerateResumeAsync(
                request.UserId,
                request.Title,
                request.TargetRole,
                request.Template,
                request.Language,
                request.Tone,
                request.SetAsActive,
                ct);

            return CreatedAtAction(nameof(GetById), new { resumeId = resume.Id }, ToDto(resume));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{resumeId:int}")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumeDto>> GetById(int resumeId, CancellationToken ct)
    {
        var resume = await _resumeRepository.GetByIdAsync(resumeId, ct);
        if (resume is null)
        {
            return NotFound();
        }

        return Ok(ToDto(resume));
    }

    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ResumeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ResumeDto>>> GetByUser(Guid userId, CancellationToken ct)
    {
        var resumes = await _resumeRepository.GetAllByUserIdAsync(userId, ct);
        return Ok(resumes.Select(ToDto));
    }

    [HttpGet("user/{userId:guid}/active")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResumeDto>> GetActive(Guid userId, [FromQuery] string targetRole, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(targetRole))
        {
            return BadRequest("targetRole is required.");
        }

        var resume = await _resumeRepository.GetActiveResumeAsync(userId, targetRole, ct);
        if (resume is null)
        {
            return NotFound();
        }

        return Ok(ToDto(resume));
    }

    [HttpPost("{resumeId:int}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(int resumeId, [FromQuery] Guid userId, CancellationToken ct)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("userId is required.");
        }

        var success = await _resumeRepository.SetActiveAsync(resumeId, userId, ct);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{resumeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int resumeId, CancellationToken ct)
    {
        await _resumeRepository.DeleteAsync(resumeId, ct);
        return NoContent();
    }

    private static ResumeDto ToDto(Resume resume) =>
        new()
        {
            Id = resume.Id,
            UserId = resume.UserId,
            Title = resume.Title,
            TargetRole = resume.TargetRole,
            Template = resume.Template,
            Language = resume.Language,
            Tone = resume.Tone,
            Version = resume.Version,
            IsActive = resume.IsActive,
            ContentHtml = resume.ContentHtml,
            ContentPlainText = resume.ContentPlainText,
            CreatedAt = resume.CreatedAt,
            LastModified = resume.LastModified
        };
}
