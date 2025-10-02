using PGB.Chat.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PGB.Chat.Application.Interfaces
{
    // Interface để giao tiếp với dịch vụ AI bên ngoài
    public interface IAiChatService
    {
        Task<string> GetChatCompletionAsync(IEnumerable<ChatMessage> history, string userPrompt);
    }
}