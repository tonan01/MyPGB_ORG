using PGB.BuildingBlocks.Domain.Exceptions;
using PGB.BuildingBlocks.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PGB.Auth.Domain.ValueObjects
{
    public class Username : ValueObject
    {
        #region Properties
        public string Value { get; }
        #endregion

        #region Constants
        public const int MinLength = 3;
        public const int MaxLength = 50;
        /// <summary>
        /// Pattern for username :text, number, underscore, dash
        /// </summary>
        private const string UsernamePattern = @"^[a-zA-Z0-9_-]+$";
        #endregion

        #region Constructors
        private Username(string value)
        {
            Value = value;
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// Create Username with validation
        /// </summary>
        public static Username Create(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException("Username không được để trống");

            username = username.Trim().ToLowerInvariant();

            if (username.Length < MinLength)
                throw new DomainException($"Username phải có ít nhất {MinLength} ký tự");

            if (username.Length > MaxLength)
                throw new DomainException($"Username không được quá {MaxLength} ký tự");

            if (!Regex.IsMatch(username, UsernamePattern))
                throw new DomainException("Username chỉ được chứa chữ cái, số, dấu gạch dưới và dấu gạch ngang");

            // Reserved usernames
            if (IsReserved(username))
                throw new DomainException($"Username '{username}' đã được hệ thống sử dụng");

            return new Username(username);
        }
        #endregion

        #region Validation Methods
        private static bool IsReserved(string username)
        {
            var reservedUsernames = new[]
            {
                "admin", "administrator", "root", "system", "api", "support",
                "help", "info", "contact", "mail", "email", "webmaster",
                "postmaster", "hostmaster", "www", "ftp", "staff", "guest"
            };

            return reservedUsernames.Contains(username.ToLowerInvariant());
        }
        #endregion

        #region ValueObject Implementation
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
        #endregion

        #region String Conversion
        public override string ToString() => Value;

        public static implicit operator string(Username username) => username.Value;
        public static explicit operator Username(string username) => Create(username);
        #endregion
    }
}
