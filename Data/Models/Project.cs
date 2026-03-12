namespace student_profile.Data.Models;

public class Project
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? LinkGitHub { get; set; }
    public string? ProjectsImages { get; set; }

    public User User { get; set; } = null!;
}
