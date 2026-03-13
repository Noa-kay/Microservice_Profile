using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;

namespace ProfilMicroservice.Mappers;

public class ChatHistoryMapperProfile : Profile
{
    public ChatHistoryMapperProfile()
    {
        CreateMap<ChatHistory, ChatHistoryDto>();
    }
}

