using PGB.BuildingBlocks.Domain.Entities;
using System;

namespace PGB.Chat.Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        public Guid ConversationId { get; private set; }
        public ChatMessageRole Role { get; private set; }
        public string Content { get; private set; } = string.Empty;

        public virtual Conversation Conversation { get; private set; } = null!;

        protected ChatMessage() { }

        public static ChatMessage Create(Guid conversationId, ChatMessageRole role, string content, string createdBy)
        {
            return new ChatMessage
            {
                ConversationId = conversationId,
                Role = role,
                Content = content,
                CreatedBy = createdBy
            };
        }
    }

    public enum ChatMessageRole
    {
        User,
        Assistant
    }
}