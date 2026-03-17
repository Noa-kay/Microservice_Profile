namespace student_profile.DTOs;

public class SkillDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
}

public class SkillToUserDto
{
    public Guid UserId { get; set; }
    public Guid SkillId { get; set; }
    public int YearsOfExperience { get; set; }
}

