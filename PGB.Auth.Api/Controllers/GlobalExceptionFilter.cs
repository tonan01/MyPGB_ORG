using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PGB.Auth.Api.Models;
using PGB.BuildingBlocks.Application.Exceptions;
using System.Linq; // Thêm using này

namespace PGB.Auth.Api.Controllers
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var traceId = context.HttpContext.TraceIdentifier;
            var errorResponse = new PGB.BuildingBlocks.WebApi.Common.Models.ApiErrorResponse
            {
                CorrelationId = traceId,
                Error = new PGB.BuildingBlocks.WebApi.Common.Models.ApiError { TraceId = traceId }
            };

            int statusCode = 500; // Mặc định là lỗi server

            switch (context.Exception)
            {
                case ValidationException vex:
                    statusCode = 400; // Bad Request
                    errorResponse.Error.Code = "VALIDATION_ERROR";
                    errorResponse.Error.Message = vex.Message;
                    errorResponse.Error.Details = vex.Errors.Select(kv => new PGB.BuildingBlocks.WebApi.Common.Models.FieldError { Field = kv.Key, Message = string.Join(",", kv.Value) }).ToList();
                    break;

                case AuthenticationException auex:
                    statusCode = 401; // Unauthorized
                    errorResponse.Error.Code = "AUTHENTICATION_FAILURE";
                    errorResponse.Error.Message = auex.Message;
                    break;

                case AuthorizationException azex:
                    statusCode = 403; // Forbidden
                    errorResponse.Error.Code = "AUTHORIZATION_FAILURE";
                    errorResponse.Error.Message = azex.Message;
                    break;

                case NotFoundException nex:
                    statusCode = 404; // Not Found
                    errorResponse.Error.Code = "RESOURCE_NOT_FOUND";
                    errorResponse.Error.Message = nex.Message;
                    break;

                default:
                    statusCode = 500; // Internal Server Error
                    errorResponse.Error.Code = "UNHANDLED_ERROR";
                    errorResponse.Error.Message = "An unexpected error occurred.";
                    // TODO: Ghi log chi tiết lỗi `context.Exception` ở đây cho môi trường Development
                    break;
            }

            context.Result = new ObjectResult(errorResponse) { StatusCode = statusCode };
            context.ExceptionHandled = true;
        }
    }
}