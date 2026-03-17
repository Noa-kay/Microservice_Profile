namespace student_profile.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? GitHubLink { get; set; }
    public string? ProjectsImages { get; set; }
}

