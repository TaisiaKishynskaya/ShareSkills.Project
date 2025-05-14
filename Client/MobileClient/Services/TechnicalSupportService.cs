using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MobileClient.Services
{
    public class TechnicalSupportService : ITechnicalSupportService
    {
        private readonly HttpClient _httpClient;
        private List<ChatMessage> _chatHistory;

        // Add HttpClient dependency for API calls
        public TechnicalSupportService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            // Initialize with welcome message
            _chatHistory = new List<ChatMessage>
            {
                new ChatMessage("Welcome to Technical Support! How can we help you today?", false, DateTime.Now.AddMinutes(-5))
            };
        }

        public async Task SendMessageAsync(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[TechnicalSupportService] Message to send: {message}");

            try
            {
                // Add user message to chat history
                _chatHistory.Add(new ChatMessage(message, true, DateTime.Now));

                // Prepare request body according to API schema
                var requestBody = new { prompt = message };

                // Make API call to Ollama
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5115/ollama/generate", requestBody);

                // Check response status
                response.EnsureSuccessStatusCode();

                // Deserialize response
                var responseData = await response.Content.ReadFromJsonAsync<OllamaResponse>();

                if (responseData != null)
                {
                    // Add AI response to chat history
                    _chatHistory.Add(new ChatMessage(responseData.response, false, DateTime.Now));
                }
                else
                {
                    // Handle empty response
                    _chatHistory.Add(new ChatMessage("Sorry, I couldn't generate a response.", false, DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TechnicalSupportService] Error: {ex.Message}");
                // Add error message to chat history
                _chatHistory.Add(new ChatMessage("Sorry, there was an error processing your request. Please try again later.", false, DateTime.Now));
            }
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync()
        {
            System.Diagnostics.Debug.WriteLine($"[TechnicalSupportService] Fetching chat history.");
            // Return a copy of the chat history
            return new List<ChatMessage>(_chatHistory);
        }
    }

    // Class to deserialize API response
    public class OllamaResponse
    {
        public string response { get; set; }
    }
}