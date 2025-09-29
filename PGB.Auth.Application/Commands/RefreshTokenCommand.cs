using PGB.BuildingBlocks.Application.Commands;

namespace PGB.Auth.Application.Commands
{
    #region Command
    public class RefreshTokenCommand : BaseCommand<RefreshTokenResponse>
    {
        #region Properties
        public string RefreshToken { get; set; } = string.Empty; 
        #endregion
    }
    #endregion

    #region Response
    public class RefreshTokenResponse
    {
        #region Properties
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; } 
        #endregion
    } 
    #endregion
}