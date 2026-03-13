using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;

namespace ProfilMicroservice.Mappers;

public class UserSkillMapperProfile : Profile
{
    public UserSkillMapperProfile()
    {
        CreateMap<SkillToUser, UserSkillDto>()
            .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UserId))
            .ForMember(d => d.SkillId, opt => opt.MapFrom(s => s.SkillId))
            .ForMember(d => d.SkillName, opt => opt.MapFrom(s => s.Skill.Name))
            .ForMember(d => d.CategoryId, opt => opt.MapFrom(s => s.Skill.CategoryId))
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Skill.Category.CategoryName))
            .ForMember(d => d.YearsOfExperience, opt => opt.MapFrom(s => s.YearsOfExperience));
    }
}

