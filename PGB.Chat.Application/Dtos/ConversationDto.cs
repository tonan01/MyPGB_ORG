using System;

namespace PGB.Chat.Application.Dtos
{
    public class ConversationDto
    {
        #region Properties
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } 
        #endregion
    }
}