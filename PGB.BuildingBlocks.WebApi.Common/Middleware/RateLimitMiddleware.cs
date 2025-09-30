using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using PGB.BuildingBlocks.WebApi.Common.Models;
using System.Text.Json;

namespace PGB.BuildingBlocks.WebApi.Common.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        // configuration via options
        private readonly Models.RateLimitOptions _options;

        public RateLimitMiddleware(RequestDelegate next, IMemoryCache cache, Models.RateLimitOptions options)
        {
            _next = next;
            _cache = cache;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var path = context.Request.Path.ToString().ToLowerInvariant();

            // Global per-IP limit
            if (!CheckLimit(GetIpKey(ip), _options.DefaultLimitPerMinute, out var ipRemaining, out var ipReset))
            {
                await TooManyRequests(context, ipRemaining, ipReset, "RATE_LIMIT_EXCEEDED");
                return;
            }

            // Additional rules for login endpoint
            if (path.Contains("/api/auth/login"))
            {
                if (!CheckLimit(GetIpKey(ip) + ":login", _options.LoginLimitPerIpPerMinute, out var remIpLogin, out var resetIpLogin))
                {
                    await TooManyRequests(context, remIpLogin, resetIpLogin, "RATE_LIMIT_EXCEEDED");
                    return;
                }

                // try extract usernameOrEmail from JSON body (best-effort)
                string? username = null;
                try
                {
                    context.Request.EnableBuffering();
                    using var sr = new StreamReader(context.Request.Body, leaveOpen: true);
                    var body = await sr.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        using var doc = JsonDocument.Parse(body);
                        if (doc.RootElement.TryGetProperty("usernameOrEmail", out var el))
                        {
                            username = el.GetString();
                        }
                        else if (doc.RootElement.TryGetProperty("username", out var el2))
                        {
                            username = el2.GetString();
                        }
                    }
                }
                catch
                {
                    // ignore parsing errors
                }

                if (!string.IsNullOrEmpty(username))
                {
                    var userKey = GetUserKey(username);
                    if (!CheckLimit(userKey, _options.LoginLimitPerUserPerMinute, out var remUser, out var resetUser))
                    {
                        await TooManyRequests(context, remUser, resetUser, "RATE_LIMIT_EXCEEDED");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private string GetIpKey(string ip) => $"rl:ip:{ip}";
        private string GetUserKey(string user) => $"rl:user:{user.ToLowerInvariant()}";

        private bool CheckLimit(string key, int limit, out int remaining, out DateTime reset)
        {
            var now = DateTime.UtcNow;
            var entry = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return new RateState { Count = 0, WindowReset = now.AddMinutes(1) };
            });

            lock (entry)
            {
                entry.Count++;
                remaining = Math.Max(0, limit - entry.Count);
                reset = entry.WindowReset;
                return entry.Count <= limit;
            }
        }

        private async Task TooManyRequests(HttpContext context, int remaining, DateTime reset, string code)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            var ttl = (int)(reset - DateTime.UtcNow).TotalSeconds;
            if (ttl < 0) ttl = 0;
            context.Response.Headers["Retry-After"] = ttl.ToString();

            var error = new ApiError
            {
                Code = code,
                Message = "Rate limit exceeded",
                TraceId = context.TraceIdentifier
            };

            var body = new ApiErrorResponse
            {
                Error = error,
                CorrelationId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(body);
        }

        private class RateState
        {
            public int Count { get; set; }
            public DateTime WindowReset { get; set; }
        }
    }
}


