namespace student_profile.DTOs;

public class StudentProfileDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public bool IsAcceptedToWork { get; set; }
    public List<string> Skills { get; set; } = new();
}
