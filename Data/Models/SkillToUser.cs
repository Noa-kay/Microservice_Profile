namespace student_profile.Data.Models;

public class SkillToUser
{
    public int UserId { get; set; }
    public int SkillId { get; set; }
    public int YearsOfExperience { get; set; }

    public User User { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

