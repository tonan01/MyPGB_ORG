using PGB.BuildingBlocks.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.Events
{
    /// <summary>
    /// Event ==> user deactivated (soft delete)
    /// Trigger: revoke tokens, notify other services, cleanup sessions
    /// </summary>
    public class UserDeactivatedEvent : DomainEvent
    {
        #region Properties
        public Guid UserId { get; }
        public string Username { get; }
        public string Reason { get; }
        public string DeactivatedBy { get; }
        #endregion

        #region Constructors
        public UserDeactivatedEvent(Guid userId, string username, string reason, string deactivatedBy)
        {
            UserId = userId;
            Username = username;
            Reason = reason;
            DeactivatedBy = deactivatedBy;
        } 
        #endregion
    }
}
