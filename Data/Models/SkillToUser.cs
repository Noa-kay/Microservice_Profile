namespace student_profile.Data.Models;

public class SkillToUser
{
    public Guid UserId { get; set; }
    public Guid SkillId { get; set; }
    public int YearsOfExperience { get; set; }

    public User User { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

