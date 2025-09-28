using PGB.BuildingBlocks.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        #region Properties
        public string Value { get; }
        #endregion

        #region Constructors
        private Email(string value)
        {
            Value = value;
        }
        #endregion

        #region Factory Methods
        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");

            if (!IsValidEmail(email))
                throw new DomainException($"Invalid email format: {email}");

            return new Email(email.ToLowerInvariant().Trim());
        }
        #endregion

        #region Validation
        private static bool IsValidEmail(string email)
        {
            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        #endregion

        #region Equality
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
        #endregion

        #region Overrides
        public override string ToString() => Value;
        #endregion

        #region Operators
        public static implicit operator string(Email email) => email.Value;
        public static explicit operator Email(string email) => Create(email);
        #endregion
    }
}
