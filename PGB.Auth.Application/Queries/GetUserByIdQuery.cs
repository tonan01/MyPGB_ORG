using PGB.BuildingBlocks.Application.Queries;

namespace PGB.Auth.Application.Queries
{
    #region Query Class
    public class GetUserByIdQuery : BaseQuery<UserDto>
    {
        #region Properties
        public Guid UserId { get; set; }
        #endregion

        #region Constructors
        public GetUserByIdQuery(Guid userId)
        {
            UserId = userId;
        }
        #endregion
    }
    #endregion

    #region DTO Class
    public class UserDto
    {
        #region Properties
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsLocked { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        #endregion
    } 
    #endregion
}