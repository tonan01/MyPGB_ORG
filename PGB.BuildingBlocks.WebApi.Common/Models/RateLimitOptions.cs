namespace PGB.BuildingBlocks.WebApi.Common.Models
{
    public class RateLimitOptions
    {
        public int DefaultLimitPerMinute { get; set; } = 100;
        public int LoginLimitPerIpPerMinute { get; set; } = 5;
        public int LoginLimitPerUserPerMinute { get; set; } = 5;
    }
}


