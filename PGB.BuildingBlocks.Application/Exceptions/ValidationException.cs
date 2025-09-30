namespace PGB.BuildingBlocks.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        #region Properties
        public IDictionary<string, string[]> Errors { get; }
        #endregion

        #region Constructors
        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("Validation failed")
        {
            Errors = errors;
        } 
        #endregion
    }

}