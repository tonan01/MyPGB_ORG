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
        #region Fields
        private readonly IChatRepository _chatRepository;
        private readonly IAiChatService _aiChatService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public SendChatMessageCommandHandler(IChatRepository chatRepository, IAiChatService aiChatService, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _aiChatService = aiChatService;
            _mapper = mapper;
        }
        #endregion

        #region Methods
        public async Task<ChatMessageDto> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
        {
            Conversation conversation;

            // 1. Tìm hoặc tạo mới cuộc hội thoại
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
                conversation = Conversation.Start(request.UserId, title, $"user_{request.UserId}");
                await _chatRepository.AddConversationAsync(conversation);
            }

            // 2. Lưu tin nhắn của người dùng
            var userMessage = ChatMessage.Create(conversation.Id, ChatMessageRole.User, request.Prompt, $"user_{request.UserId}");
            conversation.AddMessage(userMessage);

            // 3. Gửi lịch sử và câu hỏi đến dịch vụ AI
            var assistantResponseContent = await _aiChatService.GetChatCompletionAsync(conversation.Messages, request.Prompt);

            // 4. Lưu tin nhắn phản hồi của AI
            var assistantMessage = ChatMessage.Create(conversation.Id, ChatMessageRole.Assistant, assistantResponseContent, "system");
            conversation.AddMessage(assistantMessage);

            // 5. Trả về tin nhắn của AI cho client
            return _mapper.Map<ChatMessageDto>(assistantMessage);
        } 
        #endregion
    }
}