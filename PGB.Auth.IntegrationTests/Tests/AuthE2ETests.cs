using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PGB.Auth.IntegrationTests.Tests;

public class AuthE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthE2ETests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Then_Login_Returns_Token()
    {
        var client = _factory.CreateClient();

        var unique = Guid.NewGuid().ToString("N").Substring(0, 8);
        var username = $"e2e_{unique}";
        var register = new
        {
            username = username,
            email = $"{username}@example.com",
            firstName = "E2E",
            lastName = "User",
            password = "P@ssw0rd123!",
            confirmPassword = "P@ssw0rd123!"
        };

        var regResp = await client.PostAsJsonAsync("/api/auth/register", register);
        var regBody = await regResp.Content.ReadAsStringAsync();
        Assert.True(regResp.IsSuccessStatusCode, $"Register failed: {regResp.StatusCode} - {regBody}");

        var login = new { usernameOrEmail = username, password = "P@ssw0rd123!" };
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", login);
        var loginBody = await loginResp.Content.ReadAsStringAsync();
        Assert.True(loginResp.IsSuccessStatusCode, $"Login failed: {loginResp.StatusCode} - {loginBody}");

        var doc = System.Text.Json.JsonDocument.Parse(loginBody);
        Assert.True(doc.RootElement.TryGetProperty("data", out var data));
        Assert.True(data.TryGetProperty("accessToken", out var at) || data.TryGetProperty("AccessToken", out at));
        var token = at.GetString();
        Assert.False(string.IsNullOrEmpty(token));
    }
}




