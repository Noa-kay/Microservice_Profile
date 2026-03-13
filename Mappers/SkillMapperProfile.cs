using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;

namespace ProfilMicroservice.Mappers;

public class SkillMapperProfile : Profile
{
    public SkillMapperProfile()
    {
        CreateMap<Skill, SkillDto>();
    }
}

