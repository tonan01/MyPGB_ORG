using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGB.Auth.Application.Commands;
using PGB.Auth.Application.Dtos;
using PGB.Auth.Application.Queries;
using PGB.BuildingBlocks.Application.Models;
using PGB.BuildingBlocks.Domain.Common;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PGB.Auth.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        #region Fields
        private readonly IMediator _mediator;
        #endregion

        #region Constructor
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        #endregion

        #region Helpers
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        } 
        #endregion

        #region Endpoints
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> Me()
        {
            var userId = GetCurrentUserId();
            var query = new GetUserByIdQuery(userId);
            var user = await _mediator.Send(query);

            var response = new Models.ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                CorrelationId = HttpContext.TraceIdentifier
            };
            return Ok(response);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileCommand command)
        {
            command.UserId = GetCurrentUserId();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Manager}")]
        public async Task<ActionResult<PagedResult<UserDto>>> GetAllUsers([FromQuery] GetUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var command = new DeleteUserCommand
            {
                Id = id,
                UserId = GetCurrentUserId()
            };
            await _mediator.Send(command);
            return NoContent();
        } 
        #endregion
    }
}