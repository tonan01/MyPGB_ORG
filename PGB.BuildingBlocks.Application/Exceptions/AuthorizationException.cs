namespace PGB.BuildingBlocks.Application.Exceptions
{
    public class AuthorizationException : ApplicationException
    {
        #region Constructors
        public AuthorizationException(string message) : base(message)
        {
        }

        public AuthorizationException()
            : base("You do not have permission to perform this action")
        {
        } 
        #endregion
    }
}