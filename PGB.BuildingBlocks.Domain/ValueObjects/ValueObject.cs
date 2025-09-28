using PGB.BuildingBlocks.Domain.Exceptions;
using PGB.BuildingBlocks.Domain.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.ValueObjects
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        #region Equality Components
        protected abstract IEnumerable<object?> GetEqualityComponents();
        #endregion

        #region Equality Overrides
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ValueObject)obj);
        }

        public bool Equals(ValueObject? other)
        {
            if (other == null)
            {
                return false;
            }

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }
        #endregion

        #region Operators
        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueObject? left, ValueObject? right)
        {
            return !Equals(left, right);
        }
        #endregion

        #region Business Rule Validation
        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleValidationException(rule);
            }
        }
        #endregion
    }
}
