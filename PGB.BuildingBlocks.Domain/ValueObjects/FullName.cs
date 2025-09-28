using PGB.BuildingBlocks.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.ValueObjects
{
    public class FullName : ValueObject
    {
        #region Properties
        public string FirstName { get; }
        public string LastName { get; }
        public string? MiddleName { get; }
        #endregion

        #region Constructors
        private FullName(string firstName, string lastName, string? middleName = null)
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
        }
        #endregion

        #region Factory Methods
        public static FullName Create(string firstName, string lastName, string? middleName = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name cannot be empty");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name cannot be empty");

            if (firstName.Length > 50)
                throw new DomainException("First name cannot exceed 50 characters");

            if (lastName.Length > 50)
                throw new DomainException("Last name cannot exceed 50 characters");

            if (middleName?.Length > 50)
                throw new DomainException("Middle name cannot exceed 50 characters");

            return new FullName(
                firstName.Trim().ToTitleCase(),
                lastName.Trim().ToTitleCase(),
                middleName?.Trim().ToTitleCase()
            );
        }
        #endregion

        #region Computed Properties
        public string DisplayName => MiddleName != null
            ? $"{FirstName} {MiddleName} {LastName}"
            : $"{FirstName} {LastName}";

        public string Initials => MiddleName != null
            ? $"{FirstName[0]}.{MiddleName[0]}.{LastName[0]}."
            : $"{FirstName[0]}.{LastName[0]}.";
        #endregion

        #region Equality
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            yield return MiddleName;
        }
        #endregion

        #region Overrides
        public override string ToString() => DisplayName;
        #endregion
    }

    #region String Extensions
    // Extension method for string formatting
    internal static class StringExtensions
    {
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLowerInvariant());
        }
    }
    #endregion
}
