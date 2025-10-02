using PGB.BuildingBlocks.Application.Queries;
using PGB.Chat.Application.Dtos;
using System.Collections.Generic;

namespace PGB.Chat.Application.Queries
{
    public class GetUserConversationsQuery : BaseQuery<List<ConversationDto>>
    {
        // UserId kế thừa từ BaseQuery
    }
}