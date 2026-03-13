using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;

namespace ProfilMicroservice.Mappers;

public class ProjectMapperProfile : Profile
{
    public ProjectMapperProfile()
    {
        CreateMap<Project, ProjectDto>();
    }
}

