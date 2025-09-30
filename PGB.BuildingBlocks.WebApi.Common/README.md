# PGB.BuildingBlocks.WebApi.Common

Shared Web API building blocks used across PGB services.

Features

- Standardized `ApiResponse` and `ApiError` models
- Global exception filter that maps domain/application exceptions to consistent error responses
- Correlation ID middleware (`X-Correlation-Id` header)
- Rate limiting middleware (in-memory, configurable)
- Extension methods `AddWebApiCommon()` and `UseWebApiCommon()` for quick wiring

Configuration
Add a `RateLimiting` section to your `appsettings.*.json` if you want to override defaults:

```json
"RateLimiting": {
  "DefaultLimitPerMinute": 100,
  "LoginLimitPerIpPerMinute": 5,
  "LoginLimitPerUserPerMinute": 5
}
```

Usage
In an ASP.NET Core service `Program.cs`:

```csharp
builder.AddWebApiCommon();
var app = builder.Build();
app.UseWebApiCommon();
```

Notes

- The rate limiter uses `IMemoryCache` by default; for multi-instance deployments use Redis and swap middleware implementation accordingly.
- Keep `EnableSensitiveDataLogging` disabled in production.
