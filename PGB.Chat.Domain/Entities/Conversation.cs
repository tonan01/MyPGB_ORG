using PGB.BuildingBlocks.Domain.Entities;
using System;
using System.Collections.Generic;

namespace PGB.Chat.Domain.Entities
{
    public class Conversation : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;

        public virtual ICollection<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();

        protected Conversation() { }

        public static Conversation Start(Guid userId, string title, string createdBy)
        {
            return new Conversation
            {
                UserId = userId,
                Title = title,
                CreatedBy = createdBy
            };
        }

        public void AddMessage(ChatMessage message)
        {
            Messages.Add(message);
        }

        public void ChangeTitle(string newTitle, string updatedBy)
        {
            Title = newTitle;
            MarkAsUpdated(updatedBy);
        }
    }
}