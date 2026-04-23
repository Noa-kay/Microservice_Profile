using Microsoft.AspNetCore.Mvc;
using student_profile.BLL;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    /// <summary>
    /// Get all images.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ImageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ImageDto>>> GetAll(CancellationToken cancellationToken)
    {
        var images = await _imageService.GetAllImagesAsync(cancellationToken);
        return Ok(images);
    }

    /// <summary>
    /// Get image by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ImageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImageDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var image = await _imageService.GetImageByIdAsync(id, cancellationToken);
        if (image is null)
        {
            return NotFound();
        }

        return Ok(image);
    }

    /// <summary>
    /// Get images for a project.
    /// </summary>
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ImageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ImageDto>>> GetByProject(Guid projectId, CancellationToken cancellationToken)
    {
        var images = await _imageService.GetImagesByProjectIdAsync(projectId, cancellationToken);
        return Ok(images);
    }

    /// <summary>
    /// Create a new image.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ImageDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ImageDto>> Create(ImageDto image, CancellationToken cancellationToken)
    {
        var created = await _imageService.AddImageAsync(image, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing image.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ImageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImageDto>> Update(Guid id, ImageDto image, CancellationToken cancellationToken)
    {
        var exists = await _imageService.ImageExistsAsync(id, cancellationToken);
        if (!exists)
        {
            return NotFound();
        }

        image.Id = id;
        var updated = await _imageService.UpdateImageAsync(image, cancellationToken);
        return Ok(updated);
    }

    /// <summary>
    /// Delete an image.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _imageService.DeleteImageAsync(id, cancellationToken);
        return NoContent();
    }
}

