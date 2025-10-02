namespace PGB.BuildingBlocks.Domain.Common
{
    /// <summary>
    /// Defines application-wide roles as constants to ensure consistency and avoid magic strings.
    /// This class is shared across all services.
    /// </summary>
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string User = "User";
    }
}