using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGB.Auth.Application.Queries;
using PGB.BuildingBlocks.Domain.Common; // Thêm using này
using System;
using System.Security.Claims; // Thêm using này

namespace PGB.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Tất cả các action trong controller này đều yêu cầu đăng nhập
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        public async Task<ActionResult> Me()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new PGB.Auth.Api.Models.ApiErrorResponse { CorrelationId = HttpContext.TraceIdentifier });
            }

            var query = new GetUserByIdQuery(userId);
            var user = await _mediator.Send(query);

            var response = new PGB.Auth.Api.Models.ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                CorrelationId = HttpContext.TraceIdentifier
            };

            return Ok(response);
        }

        [HttpGet("all")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(new { message = $"Action executed by user with roles: {string.Join(", ", User.FindAll(ClaimTypes.Role).Select(c => c.Value))}" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public IActionResult DeleteUser(Guid id)
        {
            return Ok(new { message = $"User {id} has been deleted by an Admin." });
        }
    }
}