namespace ProfilMicroservice.DTOs;

public class PersonalDetailsDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Bio { get; set; } = default!;
}

