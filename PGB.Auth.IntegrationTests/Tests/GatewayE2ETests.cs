using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace PGB.Auth.IntegrationTests.Tests;

public class GatewayE2ETests
{
    private readonly HttpClient _client = new HttpClient { BaseAddress = new System.Uri("http://localhost:7000") };

    [Fact]
    public async Task Register_Login_Me_Via_Gateway()
    {
        var unique = Guid.NewGuid().ToString("N").Substring(0, 8);
        var username = $"e2e_gw_{unique}";

        var register = new
        {
            username = username,
            email = $"{username}@example.com",
            firstName = "E2E",
            lastName = "GW",
            password = "P@ssw0rd123!",
            confirmPassword = "P@ssw0rd123!"
        };

        var regResp = await _client.PostAsJsonAsync("/api/auth/register", register);
        var regText = await regResp.Content.ReadAsStringAsync();
        Assert.True(regResp.IsSuccessStatusCode, $"Gateway register failed: {regResp.StatusCode} - {regText}");

        var login = new { usernameOrEmail = username, password = "P@ssw0rd123!" };
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", login);
        var loginText = await loginResp.Content.ReadAsStringAsync();
        Assert.True(loginResp.IsSuccessStatusCode, $"Gateway login failed: {loginResp.StatusCode} - {loginText}");

        var doc = System.Text.Json.JsonDocument.Parse(loginText);
        Assert.True(doc.RootElement.TryGetProperty("data", out var data));
        string? token = null;
        if (data.TryGetProperty("accessToken", out var at1)) token = at1.GetString();
        if (string.IsNullOrEmpty(token) && data.TryGetProperty("AccessToken", out var at2)) token = at2.GetString();
        Assert.False(string.IsNullOrEmpty(token), "Access token missing");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var meResp = await _client.GetAsync("/api/user/me");
        var meText = await meResp.Content.ReadAsStringAsync();
        Assert.True(meResp.IsSuccessStatusCode, $"Gateway /api/user/me failed: {meResp.StatusCode} - {meText}");
    }
}




