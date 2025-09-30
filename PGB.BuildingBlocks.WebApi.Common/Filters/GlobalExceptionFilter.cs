using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PGB.BuildingBlocks.WebApi.Common.Models;
using PGB.BuildingBlocks.Application.Exceptions;
using System.Linq;

namespace PGB.BuildingBlocks.WebApi.Common.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var traceId = context.HttpContext.TraceIdentifier;

            if (context.Exception is ValidationException vex)
            {
                var error = new ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Message = vex.Message,
                    Details = vex.Errors.Select(kv => new FieldError { Field = kv.Key, Message = string.Join(",", kv.Value) }).ToList(),
                    TraceId = traceId
                };

                var result = new ApiErrorResponse
                {
                    Error = error,
                    CorrelationId = traceId
                };

                context.Result = new ObjectResult(result) { StatusCode = 400 };
                context.ExceptionHandled = true;
                return;
            }

            var generic = new ApiError
            {
                Code = "UNHANDLED_ERROR",
                Message = "An unexpected error occurred",
                TraceId = traceId
            };

            var genericResult = new ApiErrorResponse
            {
                Error = generic,
                CorrelationId = traceId
            };

            context.Result = new ObjectResult(genericResult) { StatusCode = 500 };
            context.ExceptionHandled = true;
        }
    }
}


