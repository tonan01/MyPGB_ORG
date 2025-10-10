namespace PGB.BuildingBlocks.Application.Exceptions
{
    public class ApplicationValidationException : ApplicationException
    {
        #region Constructors
        public ApplicationValidationException(string message)
           : base(message) { }

        public ApplicationValidationException(string message, Exception innerException)
            : base(message, innerException) { } 
        #endregion
    }
}
