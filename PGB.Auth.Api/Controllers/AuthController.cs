using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGB.Auth.Application.Commands;
using System;
using System.Security.Claims;

namespace PGB.Auth.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            var response = new Models.ApiResponse<RegisterUserResponse>
            {
                Success = true,
                Data = result,
                CorrelationId = HttpContext.TraceIdentifier
            };
            return Created($"/api/v1/users/{result.UserId}", response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginUserCommand command)
        {
            command.IpAddress = GetClientIpAddress();
            command.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _mediator.Send(command);
            var response = new Models.ApiResponse<LoginUserResponse>
            {
                Success = true,
                Data = result,
                CorrelationId = HttpContext.TraceIdentifier,
                Message = "Login successful",
                Timestamp = DateTime.UtcNow
            };
            return Ok(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<RefreshTokenResponse>> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            command.UserId = GetCurrentUserId();
            await _mediator.Send(command);
            return Ok(new { message = "Đăng xuất thành công" });
        }

        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogoutAll()
        {
            var command = new LogoutAllCommand
            {
                UserId = GetCurrentUserId()
            };

            await _mediator.Send(command);
            return Ok(new { message = "Đã đăng xuất khỏi tất cả thiết bị" });
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private string GetClientIpAddress()
        {
            return Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}