using AutoMapper;
using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.Chat.Application.Dtos;
using PGB.Chat.Application.Interfaces;
using PGB.Chat.Domain.Entities;
using PGB.Chat.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Chat.Application.Commands.Handlers
{
    public class SendChatMessageCommandHandler : ICommandHandler<SendChatMessageCommand, ChatMessageDto>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IAiChatService _aiChatService;
        private readonly IMapper _mapper;

        public SendChatMessageCommandHandler(IChatRepository chatRepository, IAiChatService aiChatService, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _aiChatService = aiChatService;
            _mapper = mapper;
        }

        public async Task<ChatMessageDto> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
        {
            Conversation conversation;

            if (request.ConversationId.HasValue)
            {
                conversation = await _chatRepository.GetConversationByIdAsync(request.ConversationId.Value, includeMessages: true);
                if (conversation == null || conversation.UserId != request.UserId)
                {
                    throw new NotFoundException("Conversation not found or you don't have access.");
                }
            }
            else
            {
                var title = request.Prompt.Length > 50 ? request.Prompt.Substring(0, 50) + "..." : request.Prompt;
                conversation = Conversation.Start(request.UserId, title);
                await _chatRepository.AddConversationAsync(conversation);
            }

            var userMessage = ChatMessage.Create(conversation.Id, ChatMessageRole.User, request.Prompt);
            conversation.AddMessage(userMessage);

            var assistantResponseContent = await _aiChatService.GetChatCompletionAsync(conversation.Messages, request.Prompt);

            var assistantMessage = ChatMessage.Create(conversation.Id, ChatMessageRole.Assistant, assistantResponseContent);
            conversation.AddMessage(assistantMessage);

            return _mapper.Map<ChatMessageDto>(assistantMessage);
        }
    }
}