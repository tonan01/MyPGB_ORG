using PGB.BuildingBlocks.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        #region Properties
        public string Value { get; }
        #endregion

        #region Constructors
        private PhoneNumber(string value)
        {
            Value = value;
        }
        #endregion

        #region Factory Methods
        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number cannot be empty");

            var cleanNumber = CleanPhoneNumber(phoneNumber);

            if (!IsValidPhoneNumber(cleanNumber))
                throw new DomainException($"Invalid phone number format: {phoneNumber}");

            return new PhoneNumber(cleanNumber);
        }
        #endregion

        #region Validation
        private static string CleanPhoneNumber(string phoneNumber)
        {
            return Regex.Replace(phoneNumber, @"[^\d+]", "");
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Basic validation - starts with + or digit, 7-15 digits total
            const string pattern = @"^(\+?\d{1,3})?[\d]{7,14}$";
            return Regex.IsMatch(phoneNumber, pattern);
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
        public static implicit operator string(PhoneNumber phone) => phone.Value;
        public static explicit operator PhoneNumber(string phone) => Create(phone);
        #endregion
    }

}
