namespace ProfilMicroservice.DTOs;

public class UserSkillDto
{
    public Guid UserId { get; set; }
    public Guid SkillId { get; set; }
    public string SkillName { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public int? YearsOfExperience { get; set; }
}

