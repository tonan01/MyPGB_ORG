using PGB.BuildingBlocks.Domain.Events;
using PGB.BuildingBlocks.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.Entities
{
    public abstract class BaseEntity<TId> : IEquatable<BaseEntity<TId>>, IAuditable, ISoftDelete
    {
        #region Properties
        public TId Id { get; protected set; } = default!;
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }
        public string CreatedBy { get; protected set; } = string.Empty;
        public string? UpdatedBy { get; protected set; }
        public bool IsDeleted { get; protected set; } = false;
        public DateTime? DeletedAt { get; protected set; }
        public string? DeletedBy { get; protected set; }
        // RowVersion for optimistic concurrency
        public byte[]? RowVersion { get; protected set; }
        #endregion

        #region Constructors
        protected BaseEntity() { }

        protected BaseEntity(TId id)
        {
            Id = id;
        }
        #endregion

        #region Audit Methods
        public virtual void MarkAsUpdated(string updatedBy)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }

        public virtual void MarkAsDeleted(string deletedBy)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }

        public virtual void Restore(string restoredBy)
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
            MarkAsUpdated(restoredBy);
        }
        #endregion

        #region Equality
        public bool Equals(BaseEntity<TId>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BaseEntity<TId>);
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    #region Convenience Classes
    /// <summary>
    /// Default ID Guid
    /// Auto generate UUID => Create new 
    /// </summary>
    public abstract class BaseEntity : BaseEntity<Guid>
    {
        protected BaseEntity() : base(Guid.NewGuid()) { }
        protected BaseEntity(Guid id) : base(id) { }
    }
    #endregion
}