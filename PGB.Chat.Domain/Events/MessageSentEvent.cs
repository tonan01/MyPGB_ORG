using PGB.BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Chat.Domain.Events
{
    public class MessageSentEvent : DomainEvent
    {
        #region Properties
        public Guid MessageId { get; }
        public Guid ChatRoomId { get; }
        public Guid SenderId { get; }
        public string Content { get; }
        #endregion

        #region Constructor
        public MessageSentEvent(Guid messageId, Guid chatRoomId, Guid senderId, string content)
        {
            MessageId = messageId;
            ChatRoomId = chatRoomId;
            SenderId = senderId;
            Content = content;
        } 
        #endregion
    }
}
