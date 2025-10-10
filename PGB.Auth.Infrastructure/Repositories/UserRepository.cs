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
        #region Dependencies
        private readonly AuthDbContext _context; 
        #endregion

        #region Constructor
        public UserRepository(AuthDbContext context)
        {
            _context = context;
        } 
        #endregion

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
            // EF Core's change tracker handles updates automatically.
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

            var totalCount = await queryable.CountAsync(cancellationToken);
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

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }
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
            catch (DbUpdateConcurrencyException ex)
            {
                throw new PGB.BuildingBlocks.Application.Exceptions.ConcurrencyException("Concurrency conflict detected while saving changes.", ex);
            }
        }
        #endregion
    }
}