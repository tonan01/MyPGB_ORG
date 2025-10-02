using PGB.Auth.Domain.Events;
using PGB.Auth.Domain.ValueObjects;
using PGB.BuildingBlocks.Domain.Entities;
using PGB.BuildingBlocks.Domain.Exceptions;
using PGB.BuildingBlocks.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PGB.Auth.Domain.Entities
{
    public class User : AggregateRoot
    {
        #region Properties
        public Username Username { get; private set; } = null!;
        public HashedPassword PasswordHash { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public FullName FullName { get; private set; } = null!;
        public bool IsActive { get; private set; } = true;
        public bool IsEmailVerified { get; private set; } = false;
        public int FailedLoginAttempts { get; private set; } = 0;
        public DateTime? LockedUntil { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public string? LastLoginIpAddress { get; private set; }
        #endregion

        #region Navigation Properties
        public virtual ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

        // --- PHẦN CẬP NHẬT ---
        public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        // Helper property to easily get role names for JWT
        public IEnumerable<string> Roles => UserRoles.Select(ur => ur.Role.Name);
        // --- KẾT THÚC CẬP NHẬT ---

        #endregion

        // ... (Giữ nguyên toàn bộ Constructors, Factory Methods và các phương thức khác)
        #region Constructors
        protected User() { }

        private User(Username username, Email email, FullName fullName,
                    HashedPassword passwordHash, string createdBy)
        {
            Username = username;
            Email = email;
            FullName = fullName;
            PasswordHash = passwordHash;
            CreatedBy = createdBy;
        }
        #endregion

        #region Factory Methods
        public static User Register(
            Username username,
            Email email,
            FullName fullName,
            HashedPassword passwordHash,
            string createdBy = "system")
        {
            var user = new User(username, email, fullName, passwordHash, createdBy);

            user.RaiseDomainEvent(new UserRegisteredEvent(
                user.Id, username.Value, email.Value, fullName.DisplayName));

            return user;
        }
        #endregion

        #region Authentication Methods
        public void Login(string ipAddress, string userAgent, string updatedBy)
        {
            if (IsLocked)
                throw new DomainException($"Tài khoản bị khóa đến {LockedUntil:dd/MM/yyyy HH:mm}");

            if (!IsActive)
                throw new DomainException("Tài khoản đã bị vô hiệu hóa");

            FailedLoginAttempts = 0;
            LockedUntil = null;
            LastLoginAt = DateTime.UtcNow;
            LastLoginIpAddress = ipAddress;
            MarkAsUpdated(updatedBy);

            RaiseDomainEvent(new UserLoggedInEvent(Id, Username.Value, ipAddress, userAgent));
        }

        public void RecordFailedLogin(SecuritySettings securitySettings, string updatedBy)
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= securitySettings.MaxFailedAttempts)
            {
                LockedUntil = DateTime.UtcNow.AddMinutes(securitySettings.LockoutDurationMinutes);
            }

            MarkAsUpdated(updatedBy);
        }

        public bool VerifyPassword(string plainPassword, IPasswordHasher passwordHasher)
        {
            return PasswordHash.Verify(plainPassword, passwordHasher);
        }
        #endregion

        #region Password Management
        public void ChangePassword(
            string currentPassword,
            HashedPassword newPasswordHash,
            IPasswordHasher passwordHasher,
            string updatedBy)
        {
            if (!VerifyPassword(currentPassword, passwordHasher))
                throw new DomainException("Mật khẩu hiện tại không đúng");

            PasswordHash = newPasswordHash;
            MarkAsUpdated(updatedBy);

            RevokeAllRefreshTokens(updatedBy, "Password changed");
            RaiseDomainEvent(new UserPasswordChangedEvent(Id, Username.Value, false));
        }

        public void ResetPassword(HashedPassword newPasswordHash, string updatedBy)
        {
            PasswordHash = newPasswordHash;
            FailedLoginAttempts = 0;
            LockedUntil = null;
            MarkAsUpdated(updatedBy);

            RevokeAllRefreshTokens(updatedBy, "Password reset");
            RaiseDomainEvent(new UserPasswordChangedEvent(Id, Username.Value, true));
        }
        #endregion

        #region Profile Management
        public void UpdateProfile(FullName fullName, string updatedBy)
        {
            FullName = fullName;
            MarkAsUpdated(updatedBy);
        }

        public void ChangeEmail(Email newEmail, string updatedBy)
        {
            if (Email.Equals(newEmail))
                return;

            Email = newEmail;
            IsEmailVerified = false;
            MarkAsUpdated(updatedBy);
        }

        public void VerifyEmail(string updatedBy)
        {
            IsEmailVerified = true;
            MarkAsUpdated(updatedBy);
        }
        #endregion

        #region Account Management
        public void Deactivate(string reason, string deactivatedBy)
        {
            IsActive = false;
            MarkAsUpdated(deactivatedBy);

            RevokeAllRefreshTokens(deactivatedBy, "User deactivated");
            RaiseDomainEvent(new UserDeactivatedEvent(Id, Username.Value, reason, deactivatedBy));
        }

        public void Activate(string updatedBy)
        {
            IsActive = true;
            FailedLoginAttempts = 0;
            LockedUntil = null;
            MarkAsUpdated(updatedBy);
        }

        public void Unlock(string updatedBy)
        {
            FailedLoginAttempts = 0;
            LockedUntil = null;
            MarkAsUpdated(updatedBy);
        }
        #endregion

        #region Token Management
        public RefreshToken AddRefreshToken(string tokenValue, DateTime expiresAt, string createdBy)
        {
            CleanupExpiredTokens();

            var refreshToken = RefreshToken.Create(Id, tokenValue, expiresAt, createdBy);
            RefreshTokens.Add(refreshToken);
            return refreshToken;
        }

        public void RevokeAllRefreshTokens(string revokedBy, string reason = "Manual revoke")
        {
            foreach (var token in RefreshTokens.Where(t => !t.IsRevoked))
            {
                token.Revoke(revokedBy, reason);
            }
        }

        private void CleanupExpiredTokens()
        {
            var expiredTokens = RefreshTokens.Where(t => t.IsExpired).ToList();
            foreach (var token in expiredTokens)
            {
                RefreshTokens.Remove(token);
            }
        }
        #endregion

        #region Computed Properties
        public bool IsLocked => LockedUntil.HasValue && LockedUntil.Value > DateTime.UtcNow;
        public string UsernameValue => Username.Value;
        public string EmailValue => Email.Value;
        public string DisplayName => FullName.DisplayName;
        public string Initials => FullName.Initials;
        #endregion
    }
}