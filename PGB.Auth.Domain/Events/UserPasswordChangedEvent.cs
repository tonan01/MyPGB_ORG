using PGB.BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.Events
{
    /// <summary>
    /// Event => user changed password successfully
    /// Trigger: send notification email, revoke all refresh tokens, audit log
    /// </summary>
    public class UserPasswordChangedEvent : DomainEvent
    {
        #region Properties
        public Guid UserId { get; }
        public string Username { get; }
        /// <summary>
        /// forgot password flow
        /// </summary>
        public bool IsPasswordReset { get; }
        #endregion

        #region Constructors
        public UserPasswordChangedEvent(Guid userId, string username, bool isPasswordReset = false)
        {
            UserId = userId;
            Username = username;
            IsPasswordReset = isPasswordReset;
        } 
        #endregion
    }
}
