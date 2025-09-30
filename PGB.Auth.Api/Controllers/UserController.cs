using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGB.Auth.Application.Queries;

namespace PGB.Auth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult> Me()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new PGB.Auth.Api.Models.ApiErrorResponse { CorrelationId = HttpContext.TraceIdentifier });

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
    }
}


