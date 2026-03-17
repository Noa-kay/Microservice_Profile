using AutoMapper;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Project <-> ProjectDto
        CreateMap<Project, ProjectDto>().ReverseMap();

        // User -> StudentProfileDto (רק ה-Id; שאר השדות מגיעים מ-PersonalDetails/Skills)
        CreateMap<User, StudentProfileDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForAllOtherMembers(opt => opt.Ignore());

        // PersonalDetails <-> StudentProfileDto
        CreateMap<PersonalDetails, StudentProfileDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Skills, opt => opt.Ignore())
            .ReverseMap();
            
        // ניתן להוסיף כאן מיפויים נוספים ככל שהפרויקט יגדל
    }
}