using PGB.BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.Events
{
    /// <summary>
    /// Event => user registered successfully
    /// trigger: update last login, log audit, security checks
    /// </summary>
    public class UserLoggedInEvent : DomainEvent
    {
        #region Properties
        public Guid UserId { get; }
        public string Username { get; }
        /// <summary>
        /// IP address for security monitoring
        /// </summary>
        public string? IpAddress { get; }
        /// <summary>
        /// User agent for device tracking
        /// </summary>
        public string? UserAgent { get; }
        #endregion

        #region Constructors
        public UserLoggedInEvent(Guid userId, string username, string? ipAddress = null, string? userAgent = null)
        {
            UserId = userId;
            Username = username;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        } 
        #endregion
    }
}
