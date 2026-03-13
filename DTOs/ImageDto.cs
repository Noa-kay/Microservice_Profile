namespace ProfilMicroservice.DTOs;

public class ImageDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ImageName { get; set; } = default!;
    public string Url { get; set; } = default!;
}

