using Microsoft.AspNetCore.Http;
using PGB.BuildingBlocks.WebApi.Common.Models;
using System.Text.Json;

namespace PGB.BuildingBlocks.WebApi.Common.Middleware
{
    public class RedisRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RedisRateLimitStore _store;
        private readonly RateLimitOptions _options;

        public RedisRateLimitMiddleware(RequestDelegate next, RedisRateLimitStore store, RateLimitOptions options)
        {
            _next = next;
            _store = store;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var path = context.Request.Path.ToString().ToLowerInvariant();

            if (!await CheckLimitAsync(GetIpKey(ip), _options.DefaultLimitPerMinute, context))
            {
                return;
            }

            if (path.Contains("/api/auth/login"))
            {
                if (!await CheckLimitAsync(GetIpKey(ip) + ":login", _options.LoginLimitPerIpPerMinute, context))
                {
                    return;
                }

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
                        if (doc.RootElement.TryGetProperty("usernameOrEmail", out var el)) username = el.GetString();
                        else if (doc.RootElement.TryGetProperty("username", out var el2)) username = el2.GetString();
                    }
                }
                catch { }

                if (!string.IsNullOrEmpty(username))
                {
                    if (!await CheckLimitAsync(GetUserKey(username), _options.LoginLimitPerUserPerMinute, context))
                        return;
                }
            }

            await _next(context);
        }

        private string GetIpKey(string ip) => $"rl:ip:{ip}";
        private string GetUserKey(string user) => $"rl:user:{user.ToLowerInvariant()}";

        private async Task<bool> CheckLimitAsync(string key, int limit, HttpContext context)
        {
            var val = await _store.IncrementAsync(key, 60);
            var ttl = await _store.GetTtlAsync(key);
            var remaining = (int)Math.Max(0, limit - val);
            var reset = DateTime.UtcNow.Add(ttl);
            if (val > limit)
            {
                await TooManyRequests(context, remaining, reset, "RATE_LIMIT_EXCEEDED");
                return false;
            }
            return true;
        }

        private async Task TooManyRequests(HttpContext context, int remaining, DateTime reset, string code)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            var ttl = (int)(reset - DateTime.UtcNow).TotalSeconds; if (ttl < 0) ttl = 0;
            context.Response.Headers["Retry-After"] = ttl.ToString();

            var error = new ApiError { Code = code, Message = "Rate limit exceeded", TraceId = context.TraceIdentifier };
            var body = new ApiErrorResponse { Error = error, CorrelationId = context.TraceIdentifier };
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(body);
        }
    }
}


