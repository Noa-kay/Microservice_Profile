namespace student_profile.Data.Models;

public class User
{
    public Guid Id { get; set; }

    public PersonalDetails? PersonalDetails { get; set; }
    public ChatHistory? ChatHistory { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<UserFile> Files { get; set; } = new List<UserFile>();
    public ICollection<SkillToUser> SkillToUsers { get; set; } = new List<SkillToUser>();
}