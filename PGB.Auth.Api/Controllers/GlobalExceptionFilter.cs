using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PGB.Auth.Api.Models;
using PGB.BuildingBlocks.Application.Exceptions;

namespace PGB.Auth.Api.Controllers
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var traceId = context.HttpContext.TraceIdentifier;

            if (context.Exception is ValidationException vex)
            {
                var details = vex.Errors.Select(kv => new PGB.BuildingBlocks.WebApi.Common.Models.FieldError { Field = kv.Key, Message = string.Join(",", kv.Value) }).ToList();

                var error = new PGB.BuildingBlocks.WebApi.Common.Models.ApiError
                {
                    Code = "VALIDATION_ERROR",
                    Message = vex.Message,
                    Details = details,
                    TraceId = traceId
                };

                var result = new PGB.BuildingBlocks.WebApi.Common.Models.ApiErrorResponse
                {
                    Error = error,
                    CorrelationId = traceId
                };

                context.Result = new ObjectResult(result) { StatusCode = 400 };
                context.ExceptionHandled = true;
                return;
            }

            // Generic error
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


