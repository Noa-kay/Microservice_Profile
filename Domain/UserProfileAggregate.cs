namespace ProfilMicroservice.Domain;

public class UserProfileAggregate
{
    public User User { get; set; } = default!;
    public PersonalDetails PersonalDetails { get; set; } = default!;
    public IEnumerable<Project> Projects { get; set; } = new List<Project>();
    public IEnumerable<UserFile> Resumes { get; set; } = new List<UserFile>();
    public IEnumerable<SkillToUser> UserSkills { get; set; } = new List<SkillToUser>();
    public IEnumerable<Category> SkillCategories { get; set; } = new List<Category>();
    public ChatHistory? ChatHistory { get; set; }
}

