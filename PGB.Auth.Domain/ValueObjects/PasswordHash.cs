using PGB.BuildingBlocks.Domain.Exceptions;
using PGB.BuildingBlocks.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.ValueObjects
{
    public class PasswordHash : ValueObject
    {
        #region Properties
        public string Value { get; }
        public string Algorithm { get; }
        public DateTime CreatedAt { get; }
        #endregion

        #region Constructors
        private PasswordHash(string value, string algorithm)
        {
            Value = value;
            Algorithm = algorithm;
            CreatedAt = DateTime.UtcNow;
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// Create from existing hash (from database)
        /// </summary>
        public static PasswordHash FromHash(string hash, string algorithm = "BCrypt")
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new DomainException("Password hash không được để trống");

            return new PasswordHash(hash, algorithm);
        }
        #endregion

        #region ValueObject Implementation
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
            yield return Algorithm;
        }
        #endregion

        #region String Representation
        public override string ToString() => $"[{Algorithm}] ***";
        #endregion
    }
}
