using PGB.BuildingBlocks.Domain.Entities;
using System.Collections.Generic;

namespace PGB.Auth.Domain.Entities
{
    public class Role : AggregateRoot
    {
        #region Properties
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        #endregion

        #region Navigation Properties
        public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
        #endregion

        #region Constructors
        protected Role() { }

        public Role(string name, string? description = null)
        {
            Name = name;
            Description = description;
        } 
        #endregion
    }
}