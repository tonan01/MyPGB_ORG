using AutoMapper;
using PGB.Chat.Application.Dtos;
using PGB.Chat.Domain.Entities;

namespace PGB.Chat.Application.Mappings
{
    public class ChatMappingProfile : Profile
    {
        public ChatMappingProfile()
        {
            CreateMap<Conversation, ConversationDto>();
            CreateMap<ChatMessage, ChatMessageDto>();
        }
    }
}