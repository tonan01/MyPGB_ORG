using PGB.BuildingBlocks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; } = false;
        public bool IsRevoked { get; private set; } = false;

        // Navigation properties
        public User User { get; private set; } = null!;

        // Constructor for EF
        protected RefreshToken() { }

        public static RefreshToken Create(Guid userId, string token, DateTime expiresAt, string createdBy)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt,
                CreatedBy = createdBy
            };
        }

        public void Use(string updatedBy)
        {
            IsUsed = true;
            MarkAsUpdated(updatedBy);
        }

        public void Revoke(string updatedBy)
        {
            IsRevoked = true;
            MarkAsUpdated(updatedBy);
        }

        public bool IsValid => !IsUsed && !IsRevoked && ExpiresAt > DateTime.UtcNow && !IsDeleted;
    }
}
