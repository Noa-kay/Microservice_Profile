using Microsoft.AspNetCore.Http;

namespace student_profile.BLL.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file);
}

