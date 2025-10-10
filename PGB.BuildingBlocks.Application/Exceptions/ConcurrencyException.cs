using System;

namespace PGB.BuildingBlocks.Application.Exceptions
{
    public class ConcurrencyException : Exception
    {
        #region Constructors
        public ConcurrencyException() : base("A concurrency conflict occurred while saving changes. Please retry the operation.") { }

        public ConcurrencyException(string message) : base(message) { }

        public ConcurrencyException(string message, Exception inner) : base(message, inner) { } 
        #endregion
    }
}


