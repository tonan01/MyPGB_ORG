using PGB.BuildingBlocks.Domain.Entities;
using PGB.Chat.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Chat.Domain.Entities
{
    public class ChatRoomUser : BaseEntity
    {
        public Guid ChatRoomId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;
        public ChatRoomRole Role { get; private set; } = ChatRoomRole.Member;

        // Navigation properties
        public ChatRoom ChatRoom { get; private set; } = null!;

        // Constructor for EF
        protected ChatRoomUser() { }

        public static ChatRoomUser Create(Guid chatRoomId, Guid userId, string createdBy, string chatRoomName, ChatRoomRole role = ChatRoomRole.Member)
        {
            var chatRoomUser = new ChatRoomUser
            {
                ChatRoomId = chatRoomId,
                UserId = userId,
                Role = role,
                CreatedBy = createdBy
            };

            // Add domain event
            chatRoomUser.AddDomainEvent(new UserJoinedChatRoomEvent(chatRoomId, userId, chatRoomName));

            return chatRoomUser;
        }

        public void ChangeRole(ChatRoomRole newRole, string updatedBy)
        {
            Role = newRole;
            MarkAsUpdated(updatedBy);
        }
    }

    public enum ChatRoomRole
    {
        Member = 1,
        Admin = 2,
        Owner = 3
    }
}
