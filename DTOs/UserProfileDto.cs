namespace ProfilMicroservice.DTOs;

public class UserProfileDto
{
    public UserDto User { get; set; } = default!;
    public PersonalDetailsDto PersonalDetails { get; set; } = default!;
    public IEnumerable<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
    public IEnumerable<UserFileDto> Resumes { get; set; } = new List<UserFileDto>();
    public IEnumerable<UserSkillDto> Skills { get; set; } = new List<UserSkillDto>();
    public IEnumerable<CategoryDto> SkillCategories { get; set; } = new List<CategoryDto>();
    public ChatHistoryDto? ChatHistory { get; set; }
}

