using PGB.BuildingBlocks.WebApi.Common.Models;

namespace PGB.Auth.Api.Models
{
    // Aliases to preserve existing controller references while delegating implementation to the shared library
    public class ApiResponse<T> : PGB.BuildingBlocks.WebApi.Common.Models.ApiResponse<T> { }
    public class ApiErrorResponse : PGB.BuildingBlocks.WebApi.Common.Models.ApiErrorResponse { }
    public class ApiError : PGB.BuildingBlocks.WebApi.Common.Models.ApiError { }
    public class FieldError : PGB.BuildingBlocks.WebApi.Common.Models.FieldError { }
    public class RateLimitInfo : PGB.BuildingBlocks.WebApi.Common.Models.RateLimitInfo { }
}


