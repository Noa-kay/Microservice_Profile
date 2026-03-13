namespace ProfilMicroservice.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ProjectName { get; set; } = default!;
    public string GitHubLink { get; set; } = default!;
}

