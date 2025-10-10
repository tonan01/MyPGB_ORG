using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGB.Todo.Application.Commands;
using PGB.Todo.Application.Queries;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using PGB.BuildingBlocks.Domain.Interfaces;

namespace PGB.Todo.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        // Cập nhật Constructor để Inject ICurrentUserService
        public TodosController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = _currentUserService.GetCurrentUsername();
            if (Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetTodoItemsQuery query)
        {
            query.UserId = GetCurrentUserId();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTodoItemCommand command)
        {
            command.UserId = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoItemCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in URL and body do not match.");
            }

            command.UserId = GetCurrentUserId();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteTodoItemCommand
            {
                Id = id,
                UserId = GetCurrentUserId()
            };

            await _mediator.Send(command);
            return NoContent();
        }
    }
}