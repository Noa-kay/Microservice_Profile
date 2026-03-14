using Microsoft.AspNetCore.Mvc;
using student_profile.BLL.Interfaces;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // GET: /api/student/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStudentProfile(Guid id)
    {
        try
        {
            var profile = await _studentService.GetStudentProfileAsync(id);
            if (profile is null)
            {
                return NotFound();
            }

            return Ok(profile);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the student profile.");
        }
    }

    // PUT: /api/student
    [HttpPut]
    public async Task<IActionResult> UpdateStudentProfile([FromBody] StudentProfileDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await _studentService.UpdateStudentProfileAsync(dto);
            if (!updated)
            {
                return NotFound();
            }

            return Ok(dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the student profile.");
        }
    }
}

