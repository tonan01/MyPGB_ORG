namespace PGB.BuildingBlocks.Application.Exceptions
{
    public abstract class ApplicationException : Exception
    {
        #region Constructors
        protected ApplicationException(string message) : base(message)
        {
        }

        protected ApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        } 
        #endregion
    }
}