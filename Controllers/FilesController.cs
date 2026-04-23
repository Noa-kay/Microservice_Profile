using Microsoft.AspNetCore.Mvc;
using student_profile.BLL;
using student_profile.DTOs;

namespace student_profile.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    /// <summary>
    /// Get all documents for a user.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<UserFileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserFileDto>>> GetUserDocuments(Guid userId, CancellationToken cancellationToken)
    {
        var files = await _fileService.GetAllDocumentsAsync(userId, cancellationToken);
        return Ok(files);
    }

    /// <summary>
    /// Upload or save a document.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Save(UserFileDto file, CancellationToken cancellationToken)
    {
        await _fileService.SaveFileAsync(file, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Update document metadata.
    /// </summary>
    [HttpPut("{documentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid documentId, UserFileDto file, CancellationToken cancellationToken)
    {
        await _fileService.UpdateDocumentDetailsAsync(documentId, file, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete a document.
    /// </summary>
    [HttpDelete("{documentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid documentId, CancellationToken cancellationToken)
    {
        await _fileService.DeleteDocumentAsync(documentId, cancellationToken);
        return NoContent();
    }
}

