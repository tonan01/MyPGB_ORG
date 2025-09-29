namespace PGB.BuildingBlocks.Application.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        #region Constructors
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string entityName, object key)
            : base($"{entityName} with ID {key} was not found")
        {
        } 
        #endregion
    }
}