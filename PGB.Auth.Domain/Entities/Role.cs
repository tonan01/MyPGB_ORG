using PGB.BuildingBlocks.Domain.Entities;
using System.Collections.Generic;

namespace PGB.Auth.Domain.Entities
{
    public class Role : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        // Navigation property
        public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        // Constructor for EF
        protected Role() { }

        public Role(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }
    }
}