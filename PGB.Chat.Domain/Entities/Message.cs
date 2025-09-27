using PGB.BuildingBlocks.Domain.Entities;
using PGB.Chat.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Chat.Domain.Entities
{
    public class Message : BaseEntity
    {
        public string Content { get; private set; } = string.Empty;
        public Guid ChatRoomId { get; private set; }
        public Guid SenderId { get; private set; } // Reference to user from Auth service
        public MessageType Type { get; private set; } = MessageType.Text;

        // Navigation properties
        public ChatRoom ChatRoom { get; private set; } = null!;

        // Constructor for EF
        protected Message() { }

        public static Message Create(string content, Guid chatRoomId, Guid senderId,
            string createdBy, MessageType type = MessageType.Text)
        {
            var message = new Message
            {
                Content = content,
                ChatRoomId = chatRoomId,
                SenderId = senderId,
                Type = type,
                CreatedBy = createdBy
            };

            // Add domain event
            message.AddDomainEvent(new MessageSentEvent(message.Id, chatRoomId, senderId, content));

            return message;
        }

        public void Update(string content, string updatedBy)
        {
            Content = content;
            MarkAsUpdated(updatedBy);
        }
    }

    public enum MessageType
    {
        Text = 1,
        Image = 2,
        File = 3,
        System = 4
    }
}
