using PGB.BuildingBlocks.Domain.Exceptions;
using System.Collections.Generic;

namespace PGB.BuildingBlocks.Domain.ValueObjects
{
    public class FullName : ValueObject
    {
        #region Properties
        public string FirstName { get; }
        public string LastName { get; }
        #endregion

        #region Constructors
        private FullName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        #endregion

        #region Factory Methods
        public static FullName Create(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name cannot be empty");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name cannot be empty");

            if (firstName.Length > 50)
                throw new DomainException("First name cannot exceed 50 characters");

            if (lastName.Length > 50)
                throw new DomainException("Last name cannot exceed 50 characters");

            return new FullName(
                firstName.Trim().ToTitleCase(),
                lastName.Trim().ToTitleCase()
            );
        }
        #endregion

        #region Computed Properties
        public string DisplayName => $"{FirstName} {LastName}";

        public string Initials => $"{FirstName[0]}{LastName[0]}";
        #endregion

        #region Equality
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
        #endregion

        #region Overrides
        public override string ToString() => DisplayName;
        #endregion
    }

    internal static class StringExtensions
    {
        #region ToTitleCase Extension Method
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLowerInvariant());
        } 
        #endregion
    }
}