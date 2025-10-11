using MediatR;
using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Application.Behaviors;
using PGB.BuildingBlocks.Domain.Interfaces;
using PGB.BuildingBlocks.Infrastructure.Data;
using PGB.Todo.Domain.Entities;

namespace PGB.Todo.Infrastructure.Persistence
{
    public class TodoDbContext : BaseDbContext, IUnitOfWork
    {
        #region Constructor
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public TodoDbContext(
            DbContextOptions<TodoDbContext> options,
            IMediator mediator,
            ICurrentUserService currentUserService)
            : base(options, mediator, currentUserService)
        {
        }
        #endregion

        #region DbSets
        public DbSet<TodoItem> TodoItems { get; set; }
        #endregion

        #region Model Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => e.UserId);
            });
        }
        #endregion
    }
}