using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGB.Todo.Application.Commands;
using PGB.Todo.Application.Queries;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PGB.Todo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TodosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = new GetTodoItemsQuery { UserId = GetCurrentUserId() };
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

        // --- PHẦN MỚI: ENDPOINT SỬA ---
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTodoItemCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in URL and body do not match.");
            }

            command.UserId = GetCurrentUserId();
            await _mediator.Send(command);
            return NoContent(); // 204 No Content là response thành công cho một request Update
        }
        // --- KẾT THÚC PHẦN MỚI ---


        // --- PHẦN MỚI: ENDPOINT XÓA ---
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteTodoItemCommand
            {
                Id = id,
                UserId = GetCurrentUserId()
            };

            await _mediator.Send(command);
            return NoContent(); // 204 No Content là response thành công cho một request Delete
        }
        // --- KẾT THÚC PHẦN MỚI ---
    }
}