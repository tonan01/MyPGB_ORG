using PGB.BuildingBlocks.Domain.Exceptions;
using PGB.BuildingBlocks.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.ValueObjects
{
    /// <summary>
    /// Represents a hashed password.
    /// Plain text passwords should never be stored.
    /// </summary>
    public class HashedPassword : ValueObject
    {
        #region Properties
        public string Hash { get; }
        public string? Salt { get; }
        public string Algorithm { get; }
        public DateTime CreatedAt { get; }
        #endregion

        #region Constants
        public const string DefaultAlgorithm = "BCrypt";
        #endregion

        #region Constructors
        private HashedPassword(string hash, string algorithm, string? salt = null)
        {
            Hash = hash;
            Algorithm = algorithm;
            Salt = salt;
            CreatedAt = DateTime.UtcNow;
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// Creates a HashedPassword from an existing hash.
        /// Use when loading from the database.
        /// </summary>
        public static HashedPassword FromHash(string hash, string algorithm = DefaultAlgorithm, string? salt = null)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new DomainException("Password hash không được để trống");

            if (string.IsNullOrWhiteSpace(algorithm))
                throw new DomainException("Algorithm không được để trống");

            return new HashedPassword(hash, algorithm, salt);
        }

        /// <summary>
        /// Create HashedPassword from plain text (Only to the within domain service)
        /// </summary>
        public static HashedPassword Create(string plainPassword, IPasswordHasher passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new DomainException("Password không được để trống");

            var (hash, salt) = passwordHasher.Hash(plainPassword);
            return new HashedPassword(hash, passwordHasher.Algorithm, salt);
        }
        #endregion

        #region Verification
        public bool Verify(string plainPassword, IPasswordHasher passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                return false;

            return passwordHasher.Verify(plainPassword, Hash, Salt);
        }

        /// <summary>
        /// Checks if rehashing is required (old algorithm or weak parameters)
        /// </summary>
        public bool NeedsRehash(IPasswordHasher passwordHasher)
        {
            return passwordHasher.NeedsRehash(Hash, Algorithm);
        }
        #endregion

        #region ValueObject Implementation
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Hash;
            yield return Algorithm;
            yield return Salt;
        }
        #endregion

        #region String Representation
        /// <summary>
        /// ToString no show hash for security
        /// </summary>
        public override string ToString() => $"[{Algorithm}] ***";
        #endregion
    }

    #region Password Hasher Interface
    public interface IPasswordHasher
    {
        string Algorithm { get; }
        (string hash, string? salt) Hash(string plainPassword);
        bool Verify(string plainPassword, string hash, string? salt);
        bool NeedsRehash(string hash, string algorithm);
    } 
    #endregion
}
