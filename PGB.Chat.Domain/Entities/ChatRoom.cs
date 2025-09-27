using PGB.BuildingBlocks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Chat.Domain.Entities
{
    public class ChatRoom : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Guid CreatedBy { get; private set; }
        public bool IsPrivate { get; private set; } = false;

        public ICollection<Message> Messages { get; private set; } = new List<Message>();
        public ICollection<ChatRoomUser> ChatRoomUsers { get; private set; } = new List<ChatRoomUser>();

        protected ChatRoom() { }

        public static ChatRoom Create(string name, string description, Guid createdBy, bool isPrivate =false)
        {
            return new ChatRoom
            {
                Name = name,
                Description = description,
                CreatedBy = createdBy,
                IsPrivate = isPrivate
            };
        }

        public void Update(string name, string description, string updateBy)
        {
            Name = name;
            Description = description;
            MarkAsUpdated(updateBy);
        }
    }
}
