namespace student_profile.DTOs;

public class UserFileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public string? FilePath { get; set; }
}

