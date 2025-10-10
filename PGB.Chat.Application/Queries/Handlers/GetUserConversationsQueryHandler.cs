using AutoMapper;
using PGB.BuildingBlocks.Application.Queries;
using PGB.Chat.Application.Dtos;
using PGB.Chat.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Chat.Application.Queries.Handlers
{
    public class GetUserConversationsQueryHandler : IQueryHandler<GetUserConversationsQuery, List<ConversationDto>>
    {
        #region Fields
        private readonly IChatRepository _chatRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public GetUserConversationsQueryHandler(IChatRepository chatRepository, IMapper mapper)
        {
            _chatRepository = chatRepository;
            _mapper = mapper;
        }
        #endregion

        #region Methods
        public async Task<List<ConversationDto>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
        {
            var conversations = await _chatRepository.GetConversationsByUserIdAsync(request.UserId);
            return _mapper.Map<List<ConversationDto>>(conversations);
        } 
        #endregion
    }
}