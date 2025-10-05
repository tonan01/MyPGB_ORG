using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGB.Chat.Application.Commands;
using PGB.Chat.Application.Queries;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PGB.Chat.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageCommand command)
        {
            command.UserId = GetCurrentUserId();
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var query = new GetUserConversationsQuery { UserId = GetCurrentUserId() };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("conversations/{conversationId:guid}")]
        public async Task<IActionResult> GetConversationHistory(Guid conversationId)
        {
            var query = new GetConversationHistoryQuery
            {
                UserId = GetCurrentUserId(),
                ConversationId = conversationId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}