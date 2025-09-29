using PGB.Auth.Application.Queries;
using PGB.Auth.Domain.Entities;
using PGB.BuildingBlocks.Application.Models;

namespace PGB.Auth.Application.Repositories
{
    #region User Repository Interface
    public interface IUserRepository
    {
        #region Methods
        // Basic CRUD
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteAsync(User user, CancellationToken cancellationToken = default);

        // Queries
        Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PagedResult<User>> GetPagedAsync(GetUsersQuery query, CancellationToken cancellationToken = default);

        // Refresh tokens
        Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

        // Unit of work
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Existence checks
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default); 
        #endregion
    }
    #endregion
}