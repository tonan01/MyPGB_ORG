using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PGB.Auth.IntegrationTests.Tests;

public class AuthFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Login_Me_Flow_Works()
    {
        var client = _factory.CreateClient();

        var unique = Guid.NewGuid().ToString("N").Substring(0, 8);
        var register = new
        {
            username = $"inttestuser_{unique}",
            email = $"inttest_{unique}@example.com",
            firstName = "Int",
            lastName = "Test",
            password = "P@ssw0rd123!",
            confirmPassword = "P@ssw0rd123!"
        };

        var regResp = await client.PostAsJsonAsync("/api/auth/register", register);
        if (!regResp.IsSuccessStatusCode)
        {
            var text = await regResp.Content.ReadAsStringAsync();
            throw new System.Exception($"Register failed: {regResp.StatusCode} - {text}");
        }

        var login = new { usernameOrEmail = register.username, password = "P@ssw0rd123!" };
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", login);
        if (!loginResp.IsSuccessStatusCode)
        {
            var text = await loginResp.Content.ReadAsStringAsync();
            throw new System.Exception($"Login failed: {loginResp.StatusCode} - {text}");
        }

        var loginDoc = await loginResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        string? accessToken = null;
        if (loginDoc.ValueKind == System.Text.Json.JsonValueKind.Object
            && loginDoc.TryGetProperty("data", out var dataEl)
            && dataEl.ValueKind == System.Text.Json.JsonValueKind.Object
            && dataEl.TryGetProperty("accessToken", out var atEl))
        {
            accessToken = atEl.GetString();
        }
        if (string.IsNullOrEmpty(accessToken)) throw new System.Exception("Access token missing in login response");
        if (string.IsNullOrEmpty(accessToken)) throw new System.Exception("Access token missing in login response");

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var meResp = await client.GetAsync("/api/user/me");
        meResp.EnsureSuccessStatusCode();
    }
}


