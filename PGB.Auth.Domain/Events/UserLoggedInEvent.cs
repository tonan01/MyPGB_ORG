using PGB.BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.Events
{
    public class UserLoggedInEvent : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public Guid UserId { get; }
        public string Username { get; }

        public UserLoggedInEvent(Guid userId, string username)
        {
            UserId = userId;
            Username = username;
        }
    }
}
