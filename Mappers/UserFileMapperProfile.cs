using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;

namespace ProfilMicroservice.Mappers;

public class UserFileMapperProfile : Profile
{
    public UserFileMapperProfile()
    {
        CreateMap<UserFile, UserFileDto>();
    }
}

