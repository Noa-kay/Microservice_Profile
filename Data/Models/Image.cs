namespace student_profile.Data.Models;

public class Image
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ImageName { get; set; } = string.Empty;
    public string URL { get; set; } = string.Empty;

    public Project Project { get; set; } = null!;
}