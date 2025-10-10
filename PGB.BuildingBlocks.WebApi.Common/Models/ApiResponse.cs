using System;
using System.Collections.Generic;

namespace PGB.BuildingBlocks.WebApi.Common.Models
{
    #region Success Response Model
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = "Operation completed successfully";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CorrelationId { get; set; } = string.Empty;
    } 
    #endregion

    #region Error Response Models
    public class ApiErrorResponse
    {
        public bool Success { get; set; } = false;
        public ApiError Error { get; set; } = new ApiError();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CorrelationId { get; set; } = string.Empty;
    }

    public class ApiError
    {
        public string Code { get; set; } = "UNHANDLED_ERROR";
        public string Message { get; set; } = string.Empty;
        public List<FieldError>? Details { get; set; }
        public string? TraceId { get; set; }
        public string? HelpUrl { get; set; }
    }

    public class FieldError
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    } 
    #endregion
}


