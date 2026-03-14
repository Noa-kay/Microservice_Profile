namespace student_profile.Data.Models;

public class UserFile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public string? FilePath { get; set; }
    public byte[]? FileData { get; set; }

    public User User { get; set; } = null!;
}