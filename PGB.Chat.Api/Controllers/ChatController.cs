using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PGB.BuildingBlocks.Domain.Interfaces;
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
        #region Fields
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region Constructor
        public ChatController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }
        #endregion

        #region Private Methods
        private Guid GetCurrentUserId()
        {
            var userIdString = _currentUserService.GetCurrentUsername();
            if (Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }
        #endregion

        #region Public Methods
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
        #endregion
    }
}