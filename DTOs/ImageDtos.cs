namespace student_profile.DTOs;

public class ImageDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ImageName { get; set; } = string.Empty;
    public string URL { get; set; } = string.Empty;
}

