using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;

namespace ProfilMicroservice.Mappers;

public class UserProfileMapperProfile : Profile
{
    public UserProfileMapperProfile()
    {
        CreateMap<UserProfileAggregate, UserProfileDto>()
            .ForMember(d => d.User, opt => opt.MapFrom(s => s.User))
            .ForMember(d => d.PersonalDetails, opt => opt.MapFrom(s => s.PersonalDetails))
            .ForMember(d => d.Projects, opt => opt.MapFrom(s => s.Projects))
            .ForMember(d => d.Resumes, opt => opt.MapFrom(s => s.Resumes))
            .ForMember(d => d.Skills, opt => opt.MapFrom(s => s.UserSkills))
            .ForMember(d => d.SkillCategories, opt => opt.MapFrom(s => s.SkillCategories))
            .ForMember(d => d.ChatHistory, opt => opt.MapFrom(s => s.ChatHistory));
    }
}

