namespace ProfilMicroservice.DTOs;

public class UserFileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool Uploaded { get; set; }
    public DateTime? UploadDate { get; set; }
    public string FileType { get; set; } = default!;
    public string Url { get; set; } = default!;
}

