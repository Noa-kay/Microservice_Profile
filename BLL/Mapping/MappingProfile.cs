using AutoMapper;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User <-> UserDto
        CreateMap<User, UserDto>().ReverseMap();

        // Project <-> ProjectDto
        CreateMap<Project, ProjectDto>().ReverseMap();

        // Skill <-> SkillDto
        CreateMap<Skill, SkillDto>().ReverseMap();
        CreateMap<SkillToUser, SkillToUserDto>().ReverseMap();

        // User -> StudentProfileDto
        CreateMap<User, StudentProfileDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => 
                dest.UserId == src.Id));

        // PersonalDetails <-> PersonalDetailsDto
        CreateMap<PersonalDetails, PersonalDetailsDto>().ReverseMap();

        // PersonalDetails <-> StudentProfileDto
        CreateMap<PersonalDetails, StudentProfileDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Skills, opt => opt.Ignore())
            .ReverseMap();

        // UserFile <-> UserFileDto
        CreateMap<UserFile, UserFileDto>().ReverseMap();

        // Image <-> ImageDto
        CreateMap<Image, ImageDto>().ReverseMap();

        // ChatHistory / Message <-> DTOs
        CreateMap<ChatHistory, ChatHistoryDto>().ReverseMap();
        CreateMap<Message, MessageDto>().ReverseMap();
    }
}