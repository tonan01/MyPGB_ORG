using PGB.Chat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Chat.Application.Interfaces
{
    public interface IChatRepository
    {
        #region Methods
        Task<Conversation?> GetConversationByIdAsync(Guid id, bool includeMessages = false);
        Task<List<Conversation>> GetConversationsByUserIdAsync(Guid userId);
        Task AddConversationAsync(Conversation conversation);
        Task AddChatMessageAsync(ChatMessage message);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken); 
        #endregion
    }
}