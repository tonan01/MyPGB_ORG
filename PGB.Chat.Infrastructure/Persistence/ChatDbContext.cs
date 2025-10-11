using MediatR;
using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Application.Behaviors;
using PGB.BuildingBlocks.Domain.Interfaces;
using PGB.BuildingBlocks.Infrastructure.Data;
using PGB.Chat.Domain.Entities;

namespace PGB.Chat.Infrastructure.Persistence
{
    public class ChatDbContext : BaseDbContext, IUnitOfWork
    {
        #region Constructor
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }
        public ChatDbContext(
            DbContextOptions<ChatDbContext> options,
            IMediator mediator,
            ICurrentUserService currentUserService)
            : base(options, mediator, currentUserService)
        {
        }
        #endregion

        #region DbSets
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        #endregion

        #region Model Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
                entity.HasIndex(c => c.UserId);
                entity.HasMany(c => c.Messages)
                      .WithOne(m => m.Conversation)
                      .HasForeignKey(m => m.ConversationId);
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Content).IsRequired();
            });
        }
        #endregion
    }
}