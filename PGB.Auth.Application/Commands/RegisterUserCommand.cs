using PGB.BuildingBlocks.Application.Commands;
using PGB.BuildingBlocks.Domain.ValueObjects;
using PGB.Auth.Domain.ValueObjects;

namespace PGB.Auth.Application.Commands
{
    #region Command
    public class RegisterUserCommand : BaseCommand<RegisterUserResponse>
    {
        #region Properties
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty; 
        #endregion
    } 
    #endregion

    #region Response
    public class RegisterUserResponse
    {
        #region Properties
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool RequireEmailVerification { get; set; }
        public DateTime CreatedAt { get; set; } 
        #endregion
    } 
    #endregion
}