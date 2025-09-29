namespace PGB.Auth.Application.Services
{
    #region User Domain Service Interface
    public interface IUserDomainService
    {
        Task<bool> IsUsernameAvailableAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> IsEmailAvailableAsync(string email, CancellationToken cancellationToken = default);
        bool IsPasswordStrong(string password);
        string GenerateRefreshToken();
    }
    #endregion
}