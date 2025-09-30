using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PGB.BuildingBlocks.WebApi.Common.Middleware;
using PGB.BuildingBlocks.WebApi.Common.Models;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PGB.BuildingBlocks.WebApi.Common.Tests
{
    public class RateLimitMiddlewareTests
    {
        [Fact]
        public async Task Allows_requests_under_limit()
        {
            var memory = new MemoryCache(new MemoryCacheOptions());
            var options = new RateLimitOptions { DefaultLimitPerMinute = 10 };

            var middleware = new RateLimitMiddleware(async (ctx) =>
            {
                ctx.Response.StatusCode = 200;
                await ctx.Response.WriteAsync("ok");
            }, memory, options);

            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");

            for (int i = 0; i < 5; i++)
            {
                var ms = new MemoryStream();
                context.Response.Body = ms;
                await middleware.InvokeAsync(context);
                ms.Position = 0;
            }
        }
    }
}


