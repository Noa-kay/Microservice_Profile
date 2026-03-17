namespace student_profile.Data.Models;

public class Skill
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<SkillToUser> SkillToUsers { get; set; } = new List<SkillToUser>();
}

