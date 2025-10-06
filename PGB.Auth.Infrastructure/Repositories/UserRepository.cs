using Microsoft.EntityFrameworkCore;
using PGB.Auth.Domain.Entities;
using PGB.Auth.Application.Repositories;
using PGB.Auth.Application.Queries;
using PGB.Auth.Infrastructure.Data;
using PGB.BuildingBlocks.Application.Models;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PGB.Auth.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        #region Basic CRUD Operations
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username.Value == username.ToLowerInvariant(), cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.RefreshTokens)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email.Value == email.ToLowerInvariant(), cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }
        #endregion

        #region Role Operations
        public async Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        }
        #endregion

        #region Query Operations
        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .OrderBy(u => u.Username.Value)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<User>> GetPagedAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
        {
            query.ValidateAndNormalize();
            var queryable = _context.Users.AsQueryable();

            if (query.IsActive.HasValue)
                queryable = queryable.Where(u => u.IsActive == query.IsActive.Value);

            if (query.IsEmailVerified.HasValue)
                queryable = queryable.Where(u => u.IsEmailVerified == query.IsEmailVerified.Value);

            if (query.IsLocked.HasValue)
            {
                var now = DateTime.UtcNow;
                if (query.IsLocked.Value)
                    queryable = queryable.Where(u => u.LockedUntil.HasValue && u.LockedUntil.Value > now);
                else
                    queryable = queryable.Where(u => !u.LockedUntil.HasValue || u.LockedUntil.Value <= now);
            }

            if (query.CreatedFrom.HasValue)
                queryable = queryable.Where(u => u.CreatedAt >= query.CreatedFrom.Value);

            if (query.CreatedTo.HasValue)
                queryable = queryable.Where(u => u.CreatedAt <= query.CreatedTo.Value);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLowerInvariant();
                queryable = queryable.Where(u =>
                    u.Username.Value.Contains(searchTerm) ||
                    u.Email.Value.Contains(searchTerm) ||
                    u.FullName.FirstName.Contains(searchTerm) ||
                    u.FullName.LastName.Contains(searchTerm));
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                queryable = query.SortBy.ToLowerInvariant() switch
                {
                    "username" => query.SortDescending ? queryable.OrderByDescending(u => u.Username.Value) : queryable.OrderBy(u => u.Username.Value),
                    "email" => query.SortDescending ? queryable.OrderByDescending(u => u.Email.Value) : queryable.OrderBy(u => u.Email.Value),
                    "fullname" => query.SortDescending ? queryable.OrderByDescending(u => u.FullName.FirstName).ThenByDescending(u => u.FullName.LastName) : queryable.OrderBy(u => u.FullName.FirstName).ThenBy(u => u.FullName.LastName),
                    "createdat" => query.SortDescending ? queryable.OrderByDescending(u => u.CreatedAt) : queryable.OrderBy(u => u.CreatedAt),
                    _ => queryable.OrderBy(u => u.Username.Value)
                };
            }
            else
            {
                queryable = queryable.OrderBy(u => u.Username.Value);
            }

            var users = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<User>(users, totalCount, query.Page, query.PageSize);
        }
        #endregion

        #region Refresh Token Operations
        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        // --- BẮT ĐẦU CẬP NHẬT ---
        public void AddRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }
        // --- KẾT THÚC CẬP NHẬT ---
        #endregion

        #region Existence Checks
        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(u => u.Username.Value == username.ToLowerInvariant(), cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.Value == email.ToLowerInvariant(), cancellationToken);
        }
        #endregion

        #region Unit of Work
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var entries = _context.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted || e.State == EntityState.Added)
                    .ToList();

                foreach (var entry in entries)
                {
                    try
                    {
                        await entry.ReloadAsync(cancellationToken);
                    }
                    catch
                    {
                        // ignore reload errors
                    }
                }

                try
                {
                    return await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException ex2)
                {
                    throw new PGB.BuildingBlocks.Application.Exceptions.ConcurrencyException("Concurrency conflict detected while saving changes.", ex2);
                }
            }
        }
        #endregion
    }
}