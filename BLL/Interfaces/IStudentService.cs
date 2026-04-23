using student_profile.DTOs;

namespace student_profile.BLL.Interfaces;

public interface IStudentService
{
    Task<StudentProfileDto?> GetStudentProfileAsync(Guid userId);
    Task<bool> UpdateStudentProfileAsync(StudentProfileDto dto);
}

