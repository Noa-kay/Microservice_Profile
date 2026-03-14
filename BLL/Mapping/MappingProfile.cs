using AutoMapper;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // מיפוי מפרויקט ל-DTO
        CreateMap<Project, ProjectDto>().ReverseMap();

        // מיפוי מפרטים אישיים ל-DTO של פרופיל
        // שימי לב: ה-DTO כולל גם שדות מה-User וגם מה-PersonalDetails
        CreateMap<PersonalDetails, StudentProfileDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ReverseMap();
            
        // ניתן להוסיף כאן מיפויים נוספים ככל שהפרויקט יגדל
    }
}