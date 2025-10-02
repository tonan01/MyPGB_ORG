using Microsoft.EntityFrameworkCore;
using PGB.Chat.Application.Interfaces;
using PGB.Chat.Domain.Entities;
using PGB.Chat.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PGB.Chat.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatDbContext _context;

        public ChatRepository(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation?> GetConversationByIdAsync(Guid id, bool includeMessages = false)
        {
            IQueryable<Conversation> query = _context.Conversations;
            if (includeMessages)
            {
                // Lấy 20 tin nhắn gần nhất để tránh quá tải
                query = query.Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(20));
            }
            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Conversation>> GetConversationsByUserIdAsync(Guid userId)
        {
            return await _context.Conversations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
                .ToListAsync();
        }

        public async Task AddConversationAsync(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
        }

        // Phương thức này không cần thiết vì EF Core tự động thêm message vào context khi ta gọi conversation.AddMessage()
        public Task AddChatMessageAsync(ChatMessage message)
        {
            _context.ChatMessages.AddAsync(message);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}