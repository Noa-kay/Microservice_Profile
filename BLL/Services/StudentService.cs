using AutoMapper;
using student_profile.BLL.Interfaces;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL.Services;

public class StudentService : IStudentService
{
    private readonly IUserRepository _userRepository;
    private readonly IPersonalDetailsRepository _personalDetailsRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly IGenericRepository<SkillToUser> _skillToUserRepository;
    private readonly IMapper _mapper;

    public StudentService(
        IUserRepository userRepository,
        IPersonalDetailsRepository personalDetailsRepository,
        ISkillRepository skillRepository,
        IGenericRepository<SkillToUser> skillToUserRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _personalDetailsRepository = personalDetailsRepository;
        _skillRepository = skillRepository;
        _skillToUserRepository = skillToUserRepository;
        _mapper = mapper;
    }

    public async Task<StudentProfileDto?> GetStudentProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        var details = await _personalDetailsRepository
            .FindAsync(pd => pd.UserId == userId);

        var detailsEntity = details.FirstOrDefault();

        var userSkills = await _skillRepository
            .FindAsync(s => s.SkillToUsers.Any(stu => stu.UserId == userId));

        var dto = detailsEntity is not null
            ? _mapper.Map<StudentProfileDto>(detailsEntity)
            : new StudentProfileDto { UserId = user.Id };

        dto.UserId = user.Id;
        dto.Skills = userSkills.Select(s => s.Name).ToList();

        return dto;
    }

    public async Task<bool> UpdateStudentProfileAsync(StudentProfileDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Email) &&
            !System.Text.RegularExpressions.Regex.IsMatch(
                dto.Email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Email format is invalid.", nameof(dto.Email));
        }

        var detailsList = await _personalDetailsRepository
            .FindAsync(pd => pd.UserId == dto.UserId);

        var details = detailsList.FirstOrDefault();
        if (details is null)
        {
            // אם אין PersonalDetails למשתמש – כרגע נחזיר false
            return false;
        }

        // מיפוי השדות הרלוונטיים מה-DTO ל-Entity
        _mapper.Map(dto, details);
        await _personalDetailsRepository.UpdateAsync(details);

        // ניהול כישורים (SkillToUsers)
        var requestedSkillNames = dto.Skills
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        // כל ה-Skills המערכת (כדי למפות שם -> Id)
        var allSkills = await _skillRepository.GetAllAsync();
        var skillNameToId = allSkills
            .GroupBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

        // כל הקישורים הקיימים עבור המשתמש
        var existingLinks = await _skillToUserRepository
            .FindAsync(stu => stu.UserId == dto.UserId);
        var existingLinksList = existingLinks.ToList();

        // שמות כישורים קיימים למשתמש
        var existingSkillNames = existingLinksList
            .Select(link =>
                skillNameToId
                    .Where(kv => kv.Value == link.SkillId)
                    .Select(kv => kv.Key)
                    .FirstOrDefault())
            .Where(name => name is not null)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // כישורים להוספה (ב-DTO אבל לא קיימים למשתמש)
        var skillsToAddNames = requestedSkillNames
            .Where(name => !existingSkillNames.Contains(name))
            .ToList();

        foreach (var skillName in skillsToAddNames)
        {
            if (!skillNameToId.TryGetValue(skillName, out var skillId))
            {
                // כישור לא קיים בטבלת Skills – כרגע מדלגים
                continue;
            }

            var newLink = new SkillToUser
            {
                UserId = dto.UserId,
                SkillId = skillId
            };

            await _skillToUserRepository.AddAsync(newLink);
        }

        // כישורים להסרה (קיימים למשתמש אך לא נמצאים יותר ב-DTO)
        var skillsToKeep = new HashSet<string>(requestedSkillNames, StringComparer.OrdinalIgnoreCase);

        foreach (var link in existingLinksList)
        {
            var name = skillNameToId
                .Where(kv => kv.Value == link.SkillId)
                .Select(kv => kv.Key)
                .FirstOrDefault();

            if (name is null)
            {
                continue;
            }

            if (!skillsToKeep.Contains(name))
            {
                await _skillToUserRepository.DeleteAsync(link);
            }
        }

        // שמירה אחת על ההקשר – כל השינויים (Details + Skills)
        await _personalDetailsRepository.SaveChangesAsync();

        return true;
    }
}

