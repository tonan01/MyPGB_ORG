using PGB.BuildingBlocks.Domain.Entities;
using PGB.BuildingBlocks.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        #region Properties
        public string Token { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; } = false;
        public bool IsRevoked { get; private set; } = false;
        public string? RevokeReason { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? RevokedBy { get; private set; }
        #endregion

        #region Navigation Properties
        public User User { get; private set; } = null!;
        #endregion

        #region Constructors
        protected RefreshToken() { }

        private RefreshToken(Guid userId, string token, DateTime expiresAt, string createdBy)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            CreatedBy = createdBy;
        }
        #endregion

        #region Factory Methods
        public static RefreshToken Create(Guid userId, string token, DateTime expiresAt, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new DomainException("Token không được để trống");

            if (expiresAt <= DateTime.UtcNow)
                throw new DomainException("Token phải có thời gian hết hạn trong tương lai");

            return new RefreshToken(userId, token, expiresAt, createdBy);
        }
        #endregion

        #region Token Operations
        public void Use(string updatedBy)
        {
            if (!IsValid)
                throw new DomainException("Token không hợp lệ");

            IsUsed = true;
            MarkAsUpdated(updatedBy);
        }

        public void Revoke(string revokedBy, string reason)
        {
            if (IsRevoked) return;

            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            RevokedBy = revokedBy;
            RevokeReason = reason;
            MarkAsUpdated(revokedBy);
        }
        #endregion

        #region Validation Properties
        public bool IsExpired => ExpiresAt <= DateTime.UtcNow;
        public bool IsValid => !IsUsed && !IsRevoked && !IsExpired && !IsDeleted;
        #endregion
    }
}
