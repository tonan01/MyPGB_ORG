using PGB.BuildingBlocks.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        #region Properties
        public decimal Amount { get; }
        public string Currency { get; }
        #endregion

        #region Constructors
        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }
        #endregion

        #region Factory Methods
        public static Money Create(decimal amount, string currency = "USD")
        {
            if (amount < 0)
                throw new DomainException("Money amount cannot be negative");

            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException("Currency cannot be empty");

            if (currency.Length != 3)
                throw new DomainException("Currency must be 3 characters (ISO 4217)");

            return new Money(Math.Round(amount, 2), currency.ToUpperInvariant());
        }

        public static Money Zero(string currency = "USD") => Create(0, currency);
        #endregion

        #region Operations
        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Cannot add different currencies: {Currency} and {other.Currency}");

            return Create(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Cannot subtract different currencies: {Currency} and {other.Currency}");

            return Create(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal multiplier)
        {
            return Create(Amount * multiplier, Currency);
        }
        #endregion

        #region Comparisons
        public bool IsGreaterThan(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Cannot compare different currencies: {Currency} and {other.Currency}");

            return Amount > other.Amount;
        }

        public bool IsLessThan(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Cannot compare different currencies: {Currency} and {other.Currency}");

            return Amount < other.Amount;
        }
        #endregion

        #region Equality
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
        #endregion

        #region Overrides
        public override string ToString() => $"{Amount:F2} {Currency}";
        #endregion

        #region Operators
        public static Money operator +(Money left, Money right) => left.Add(right);
        public static Money operator -(Money left, Money right) => left.Subtract(right);
        public static Money operator *(Money money, decimal multiplier) => money.Multiply(multiplier);
        public static bool operator >(Money left, Money right) => left.IsGreaterThan(right);
        public static bool operator <(Money left, Money right) => left.IsLessThan(right);
        #endregion
    }

}
