using System;
using PGB.Auth.Domain.ValueObjects;
using BCrypt.Net;

namespace PGB.Auth.Infrastructure.Services
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string Algorithm => "BCrypt";

        public (string hash, string? salt) Hash(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty", nameof(plainPassword));

            // Generate salt and hash
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hash = BCrypt.Net.BCrypt.HashPassword(plainPassword, salt);

            return (hash, salt);
        }

        public bool Verify(string plainPassword, string hash, string? salt)
        {
            if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(hash))
                return false;

            return BCrypt.Net.BCrypt.Verify(plainPassword, hash);
        }

        public bool NeedsRehash(string hash, string algorithm)
        {
            if (string.IsNullOrWhiteSpace(hash))
                return true;

            // If algorithm changed, require rehash
            if (!string.Equals(algorithm, Algorithm, StringComparison.OrdinalIgnoreCase))
                return true;

            // Optionally: check cost factor and decide rehash.
            // For BCrypt.Net, you can parse and inspect the work factor from the hash string if needed.
            return false;
        }
    }
}