using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using PGB.Auth.Application.Commands;
using PGB.Auth.Application.Queries;
using System;
using System.Security.Claims; // Thêm using này
using PGB.BuildingBlocks.Domain.Common; // Thêm using này

namespace PGB.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Public Endpoints
        /// <summary>
        /// Register new user
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous] // Rõ ràng cho biết endpoint này là public
        public async Task<ActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            var response = new PGB.Auth.Api.Models.ApiResponse<RegisterUserResponse>
            {
                Success = true,
                Data = result,
                CorrelationId = HttpContext.TraceIdentifier
            };
            return Created($"/api/users/{result.UserId}", response);
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginUserCommand command)
        {
            command.IpAddress = GetClientIpAddress();
            command.UserAgent = Request.Headers["User-Agent"].ToString();

            var result = await _mediator.Send(command);
            var response = new PGB.Auth.Api.Models.ApiResponse<LoginUserResponse>
            {
                Success = true,
                Data = result,
                CorrelationId = HttpContext.TraceIdentifier,
                Message = "Login successful",
                Timestamp = DateTime.UtcNow
            };
            return Ok(response);
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<RefreshTokenResponse>> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        #endregion

        #region Protected Endpoints
        /// <summary>
        /// Logout from current device (revoke single refresh token)
        /// </summary>
        [HttpPost("logout")]
        [Authorize] // Yêu cầu người dùng phải đăng nhập
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            // UserId đã được set trong command từ client hoặc có thể lấy từ claims
            command.UserId = GetCurrentUserId();
            await _mediator.Send(command);
            return Ok(new { message = "Đăng xuất thành công" });
        }

        /// <summary>
        /// Logout from all devices (revoke all refresh tokens)
        /// </summary>
        [HttpPost("logout-all")]
        [Authorize] // Yêu cầu người dùng phải đăng nhập
        public async Task<IActionResult> LogoutAll()
        {
            var command = new LogoutAllCommand
            {
                UserId = GetCurrentUserId()
            };

            await _mediator.Send(command);
            return Ok(new { message = "Đã đăng xuất khỏi tất cả thiết bị" });
        }
        #endregion

        #region Helper Methods
        private Guid GetCurrentUserId()
        {
            // Phương thức này giờ đã hoạt động đúng nhờ UserClaimsMiddleware
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private string GetClientIpAddress()
        {
            return Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
        #endregion
    }
}