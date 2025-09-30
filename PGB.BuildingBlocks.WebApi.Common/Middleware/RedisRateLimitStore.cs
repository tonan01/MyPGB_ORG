using StackExchange.Redis;
using System.Threading.Tasks;

namespace PGB.BuildingBlocks.WebApi.Common.Middleware
{
    public class RedisRateLimitStore
    {
        private readonly IDatabase _db;

        public RedisRateLimitStore(IConnectionMultiplexer mux)
        {
            _db = mux.GetDatabase();
        }

        public async Task<long> IncrementAsync(string key, int windowSeconds)
        {
            // Use INCR and set expiry if new
            var val = await _db.StringIncrementAsync(key).ConfigureAwait(false);
            if (val == 1)
            {
                await _db.KeyExpireAsync(key, TimeSpan.FromSeconds(windowSeconds)).ConfigureAwait(false);
            }
            return val;
        }

        public async Task<TimeSpan> GetTtlAsync(string key)
        {
            var ttl = await _db.KeyTimeToLiveAsync(key).ConfigureAwait(false);
            return ttl ?? TimeSpan.Zero;
        }
    }
}


