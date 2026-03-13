using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;

namespace ProfilMicroservice.Mappers;

public class ImageMapperProfile : Profile
{
    public ImageMapperProfile()
    {
        CreateMap<Image, ImageDto>();
    }
}

