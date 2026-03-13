using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;

namespace ProfilMicroservice.Mappers;

public class ChatMessageMapperProfile : Profile
{
    public ChatMessageMapperProfile()
    {
        CreateMap<Message, ChatMessageDto>()
            .ForMember(d => d.Message, opt => opt.MapFrom(s => s.Massage));
    }
}

