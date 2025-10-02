using PGB.BuildingBlocks.Application.Commands;
using PGB.Chat.Application.Dtos;
using System;

namespace PGB.Chat.Application.Commands
{
    public class SendChatMessageCommand : BaseCommand<ChatMessageDto>
    {
        // Nếu có, gửi tin nhắn vào cuộc hội thoại đã có
        public Guid? ConversationId { get; set; }

        // Nội dung câu hỏi của người dùng
        public string Prompt { get; set; } = string.Empty;

        // UserId được kế thừa từ BaseCommand
    }
}