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
        public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
        #endregion

        #region Constructors
        protected User() { }

        private User(Username username, Email email, FullName fullName, HashedPassword passwordHash)
        {
            Username = username;
            Email = email;
            FullName = fullName;
            PasswordHash = passwordHash;
        }
        #endregion

        #region Factory Methods
        public static User Register(
            Username username,
            Email email,
            FullName fullName,
            HashedPassword passwordHash)
        {
            var user = new User(username, email, fullName, passwordHash);

            user.RaiseDomainEvent(new UserRegisteredEvent(
                user.Id, username.Value, email.Value, fullName.DisplayName));

            return user;
        }
        #endregion

        #region Authentication Methods
        public void Login(string ipAddress, string userAgent)
        {
            if (IsLocked)
                throw new DomainException($"Tài khoản bị khóa đến {LockedUntil:dd/MM/yyyy HH:mm}");

            if (!IsActive)
                throw new DomainException("Tài khoản đã bị vô hiệu hóa");

            FailedLoginAttempts = 0;
            LockedUntil = null;
            LastLoginAt = DateTime.UtcNow;
            LastLoginIpAddress = ipAddress;

            RaiseDomainEvent(new UserLoggedInEvent(Id, Username.Value, ipAddress, userAgent));
        }

        public void RecordFailedLogin(SecuritySettings securitySettings)
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= securitySettings.MaxFailedAttempts)
            {
                LockedUntil = DateTime.UtcNow.AddMinutes(securitySettings.LockoutDurationMinutes);
            }
        }

        public bool VerifyPassword(string plainPassword, IPasswordHasher passwordHasher)
        {
            return PasswordHash.Verify(plainPassword, passwordHasher);
        }
        #endregion

        #region Role Management
        public void AddRole(Role role)
        {
            if (!UserRoles.Any(ur => ur.RoleId == role.Id))
            {
                UserRoles.Add(new UserRole(this.Id, role.Id));
            }
        }
        #endregion

        #region Password Management
        public void ChangePassword(
            string currentPassword,
            HashedPassword newPasswordHash,
            IPasswordHasher passwordHasher)
        {
            if (!VerifyPassword(currentPassword, passwordHasher))
                throw new DomainException("Mật khẩu hiện tại không đúng");

            PasswordHash = newPasswordHash;

            RevokeAllRefreshTokens("Password changed");
            RaiseDomainEvent(new UserPasswordChangedEvent(Id, Username.Value, false));
        }

        public void ResetPassword(HashedPassword newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            FailedLoginAttempts = 0;
            LockedUntil = null;

            RevokeAllRefreshTokens("Password reset");
            RaiseDomainEvent(new UserPasswordChangedEvent(Id, Username.Value, true));
        }
        #endregion

        #region Profile Management
        public void UpdateProfile(FullName fullName)
        {
            FullName = fullName;
        }

        public void ChangeEmail(Email newEmail)
        {
            if (Email.Equals(newEmail))
                return;

            Email = newEmail;
            IsEmailVerified = false;
        }

        public void VerifyEmail()
        {
            IsEmailVerified = true;
        }
        #endregion

        #region Account Management
        public void Deactivate(string reason)
        {
            IsActive = false;

            RevokeAllRefreshTokens("User deactivated");
            RaiseDomainEvent(new UserDeactivatedEvent(Id, Username.Value, reason, UpdatedBy ?? "system"));
        }

        public void Activate()
        {
            IsActive = true;
            FailedLoginAttempts = 0;
            LockedUntil = null;
        }

        public void Unlock()
        {
            FailedLoginAttempts = 0;
            LockedUntil = null;
        }
        #endregion

        #region Token Management
        public RefreshToken AddRefreshToken(string tokenValue, DateTime expiresAt)
        {
            CleanupExpiredTokens();
            var refreshToken = RefreshToken.Create(this.Id, tokenValue, expiresAt);
            return refreshToken;
        }

        public void RevokeAllRefreshTokens(string reason = "Manual revoke")
        {
            foreach (var token in RefreshTokens.Where(t => !t.IsRevoked))
            {
                token.Revoke(reason);
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