using PGB.Chat.Domain.Enums;
using System;

namespace PGB.Chat.Application.Dtos
{
    public class ChatMessageDto
    {
        #region Properties
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public ChatMessageRole Role { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } 
        #endregion
    }
}