using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; // THÊM USING NÀY
using PGB.Chat.Application.Interfaces;
using PGB.Chat.Domain.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using PGB.Chat.Domain.Enums;

namespace PGB.Chat.Infrastructure.Services
{
    public class OpenAiChatService : IAiChatService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        private readonly OpenAiSettings _settings;
        private readonly ILogger<OpenAiChatService>? _logger;
        #endregion

        #region Constructor
        public OpenAiChatService(
           IHttpClientFactory httpClientFactory,
           IOptions<OpenAiSettings> settings,
           ILogger<OpenAiChatService>? logger = null)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _settings = settings.Value; // LẤY CẤU HÌNH TỪ IOPSIONS

            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                throw new InvalidOperationException("OpenAI API Key not configured in appsettings.json");
            }

            if (string.IsNullOrEmpty(_settings.ModelName))
            {
                _settings.ModelName = "gpt-3.5-turbo"; // Gán giá trị mặc định nếu không có
            }

            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        }
        #endregion

        #region Methods
        public async Task<string> GetChatCompletionAsync(
           IEnumerable<ChatMessage> history,
           string userPrompt)
        {
            try
            {
                var messages = new List<object>();

                // System message
                messages.Add(new { role = "system", content = "You are a helpful assistant." });

                // History
                foreach (var msg in history)
                {
                    // Sử dụng enum ChatMessageRole đã được tách file
                    var role = msg.Role == ChatMessageRole.User ? "user" : "assistant";
                    messages.Add(new { role, content = msg.Content });
                }

                // New message
                messages.Add(new { role = "user", content = userPrompt });

                // Request body
                var requestBody = new
                {
                    model = _settings.ModelName,
                    messages,
                    temperature = 0.7,
                    max_tokens = 800
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call API
                var response = await _httpClient.PostAsync("chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OpenAiResponse>(responseJson);

                var answer = result?.Choices?[0]?.Message?.Content ?? "No response";

                _logger?.LogInformation("OpenAI request successful");

                return answer;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "HTTP error calling OpenAI API");
                throw new Exception($"AI service HTTP error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error calling OpenAI API");
                throw new Exception($"AI service error: {ex.Message}", ex);
            }
        }
        #endregion

        #region Response Models
        private class OpenAiResponse
        {
            public List<Choice>? Choices { get; set; }
        }

        private class Choice
        {
            public Message? Message { get; set; }
        }

        private class Message
        {
            public string? Content { get; set; }
        }
        #endregion
    }
}