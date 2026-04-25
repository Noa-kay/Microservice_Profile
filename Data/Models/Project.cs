namespace student_profile.Data.Models;

public class Project
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? GitHubLink { get; set; }
    public string? ProjectsImages { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Image> Images { get; set; } = new List<Image>();
}
