namespace ProfilMicroservice.DTOs;

public class SkillDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid CategoryId { get; set; }
}

