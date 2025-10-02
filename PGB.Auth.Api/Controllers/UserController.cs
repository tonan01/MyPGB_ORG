using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGB.Auth.Application.Commands; // THÊM USING NÀY
using PGB.Auth.Application.Queries;
using PGB.BuildingBlocks.Domain.Common;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PGB.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        /// <summary>
        /// Gets the profile of the currently logged-in user.
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult> Me()
        {
            var userId = GetCurrentUserId();
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

        // --- PHẦN MỚI: ENDPOINT ĐỂ USER TỰ CẬP NHẬT PROFILE ---
        /// <summary>
        /// Updates the profile of the currently logged-in user.
        /// </summary>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command)
        {
            command.UserId = GetCurrentUserId();
            await _mediator.Send(command);
            return NoContent(); // 204 No Content
        }
        // --- KẾT THÚC PHẦN MỚI ---


        /// <summary>
        /// Gets a list of all users. (Admin/Manager only)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
        public async Task<IActionResult> GetAllUsers()
        {
            // Logic lấy tất cả user sẽ được hoàn thiện sau
            return Ok(new { message = $"Action executed by user with roles: {string.Join(", ", User.FindAll(ClaimTypes.Role).Select(c => c.Value))}" });
        }

        /// <summary>
        /// Deletes a user. (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public IActionResult DeleteUser(Guid id)
        {
            // Logic để xóa người dùng sẽ được hoàn thiện sau
            return Ok(new { message = $"User {id} has been deleted by an Admin." });
        }
    }
}