using PGB.BuildingBlocks.Domain.Entities;
using System;
using System.Collections.Generic;

namespace PGB.Chat.Domain.Entities
{
    public class Conversation : AggregateRoot
    {
        #region Properties
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        #endregion

        #region Navigation Properties
        public virtual ICollection<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();

        #endregion

        #region Constructor
        protected Conversation() { }

        public static Conversation Start(Guid userId, string title)
        {
            return new Conversation
            {
                UserId = userId,
                Title = title,
            };
        }
        #endregion

        #region Methods
        public void AddMessage(ChatMessage message)
        {
            Messages.Add(message);
        }

        public void ChangeTitle(string newTitle)
        {
            Title = newTitle;
        }
        #endregion
    }
}