using AutoMapper;
using PGB.BuildingBlocks.Application.Exceptions;
using PGB.BuildingBlocks.Application.Queries;
using PGB.Chat.Application.Dtos;
using PGB.Chat.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Chat.Application.Queries.Handlers
{
    public class GetConversationHistoryQueryHandler : IQueryHandler<GetConversationHistoryQuery, List<ChatMessageDto>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;

        public GetConversationHistoryQueryHandler(IChatRepository chatRepository, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
        }

        public async Task<List<ChatMessageDto>> Handle(GetConversationHistoryQuery request, CancellationToken cancellationToken)
        {
            var conversation = await _chatRepository.GetConversationByIdAsync(request.ConversationId, includeMessages: true);

            if (conversation == null || conversation.UserId != request.UserId)
            {
                throw new NotFoundException("Conversation not found or you don't have access.");
            }

            return _mapper.Map<List<ChatMessageDto>>(conversation.Messages.OrderBy(m => m.CreatedAt));
        }
    }
}