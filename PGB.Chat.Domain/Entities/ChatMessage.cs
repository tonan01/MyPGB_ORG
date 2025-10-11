using PGB.BuildingBlocks.Domain.Entities;
using PGB.Chat.Domain.Enums;
using System;

namespace PGB.Chat.Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        #region Properties
        public Guid ConversationId { get; private set; }
        public ChatMessageRole Role { get; private set; }
        public string Content { get; private set; } = string.Empty;
        #endregion

        #region Navigation Properties
        public virtual Conversation Conversation { get; private set; } = null!;
        #endregion

        #region Constructor
        protected ChatMessage() { }

        public static ChatMessage Create(Guid conversationId, ChatMessageRole role, string content)
        {
            return new ChatMessage
            {
                ConversationId = conversationId,
                Role = role,
                Content = content,
            };
        } 
        #endregion
    }
}