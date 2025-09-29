namespace PGB.BuildingBlocks.Application.Models
{
    #region Result Classes
    public class Result
    {
        #region Properties
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }
        #endregion

        #region Constructor
        protected Result(bool isSuccess, string? error)
        {
            if (isSuccess && error != null)
                throw new InvalidOperationException("Success result cannot have error");

            if (!isSuccess && error == null)
                throw new InvalidOperationException("Failure result must have error");

            IsSuccess = isSuccess;
            Error = error;
        }
        #endregion

        #region Factory Methods
        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);

        public static Result<T> Success<T>(T value) => new(value, true, null);
        public static Result<T> Failure<T>(string error) => new(default!, false, error); 
        #endregion
    }
    #endregion

    #region Generic Result Classes
    public class Result<T> : Result
    {
        #region Properties
        public T? Value { get; }
        #endregion

        #region Constructor
        protected internal Result(T? value, bool isSuccess, string? error)
           : base(isSuccess, error)
        {
            Value = value;
        } 
        #endregion
    } 
    #endregion
}