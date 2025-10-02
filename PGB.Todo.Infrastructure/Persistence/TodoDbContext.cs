using Microsoft.EntityFrameworkCore;
using PGB.BuildingBlocks.Infrastructure.Data;
using PGB.Todo.Domain.Entities;

namespace PGB.Todo.Infrastructure.Persistence
{
    public class TodoDbContext : BaseDbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }

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
    }
}