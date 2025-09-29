using PGB.BuildingBlocks.Application.Commands;

namespace PGB.Auth.Application.Commands
{
    #region Logout Command
    public class LogoutCommand : BaseCommand
    {
        #region Properties
        /// <summary>
        /// Refresh token cần revoke
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty; 
        #endregion
    }
    #endregion
}