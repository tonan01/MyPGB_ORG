using PGB.BuildingBlocks.Application.Queries;
using PGB.Chat.Application.Dtos;
using System;
using System.Collections.Generic;

namespace PGB.Chat.Application.Queries
{
    public class GetConversationHistoryQuery : BaseQuery<List<ChatMessageDto>>
    {
        public Guid ConversationId { get; set; }
    }
}