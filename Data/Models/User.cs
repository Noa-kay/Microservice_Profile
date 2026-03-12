namespace student_profile.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Bio { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<SkillToUser> SkillToUsers { get; set; } = new List<SkillToUser>();
    public ICollection<ChatHistory> ChatHistories { get; set; } = new List<ChatHistory>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<UserFile> Files { get; set; } = new List<UserFile>();
}
