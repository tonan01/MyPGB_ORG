using PGB.Auth.Domain.Events;
using PGB.BuildingBlocks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;
        public DateTime? LastLoginAt { get; private set; }

        // Navigation properties
        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

        // Constructor for EF
        protected User() { }

        // Factory method
        public static User Create(string username, string passwordHash, string email,
            string firstName, string lastName, string createdBy)
        {
            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedBy = createdBy
            };

            // Add domain event
            user.AddDomainEvent(new UserRegisteredEvent(user.Id, username, email));

            return user;
        }

        public void UpdateProfile(string firstName, string lastName, string email, string updatedBy)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            MarkAsUpdated(updatedBy);
        }

        public void UpdatePassword(string newPasswordHash, string updatedBy)
        {
            PasswordHash = newPasswordHash;
            MarkAsUpdated(updatedBy);
        }

        public void RecordLogin(string updatedBy)
        {
            LastLoginAt = DateTime.UtcNow;
            MarkAsUpdated(updatedBy);

            // Add domain event
            AddDomainEvent(new UserLoggedInEvent(Id, Username));
        }

        public void Deactivate(string deletedBy)
        {
            IsActive = false;
            MarkAsDeleted(deletedBy);
        }
    }
}
