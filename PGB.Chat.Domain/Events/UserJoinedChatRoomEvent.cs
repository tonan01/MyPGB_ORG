using PGB.BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Chat.Domain.Events
{
    public class UserJoinedChatRoomEvent : DomainEvent
    {
        #region Properties
        public Guid ChatRoomId { get; }
        public Guid UserId { get; }
        public string ChatRoomName { get; }
        #endregion

        #region Constructors
        public UserJoinedChatRoomEvent(Guid chatRoomId, Guid userId, string chatRoomName)
        {
            ChatRoomId = chatRoomId;
            UserId = userId;
            ChatRoomName = chatRoomName;
        } 
        #endregion
    }
}
