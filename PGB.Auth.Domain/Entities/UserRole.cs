using PGB.BuildingBlocks.Domain.Entities;
using System;

namespace PGB.Auth.Domain.Entities
{
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }

        // Navigation properties
        public virtual User User { get; private set; } = null!;
        public virtual Role Role { get; private set; } = null!;

        // Constructor for EF
        protected UserRole() { }

        public UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}