namespace PGB.BuildingBlocks.Application.Exceptions
{
    public class AuthenticationException : ApplicationException
    {
        #region Constructors
        public AuthenticationException(string message) : base(message)
        {
        } 
        #endregion
    }
}