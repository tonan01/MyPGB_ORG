using PGB.BuildingBlocks.Domain.Entities;
using System;

namespace PGB.Auth.Domain.Entities
{
    public class UserRole : BaseEntity
    {
        #region Properties
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; } 
        #endregion

        #region Navigation Properties
        public virtual User User { get; private set; } = null!;
        public virtual Role Role { get; private set; } = null!;
        #endregion

        #region Constructor
        protected UserRole() { }

        public UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        } 
        #endregion
    }
}