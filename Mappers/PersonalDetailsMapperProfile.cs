using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;

namespace ProfilMicroservice.Mappers;

public class PersonalDetailsMapperProfile : Profile
{
    public PersonalDetailsMapperProfile()
    {
        CreateMap<PersonalDetails, PersonalDetailsDto>();
    }
}

