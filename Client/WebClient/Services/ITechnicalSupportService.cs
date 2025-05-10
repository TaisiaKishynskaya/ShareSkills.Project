namespace WebClient.Services
{
    // A simple record to represent a chat message
    public record ChatMessage(string Text, bool IsUserMessage, DateTime Timestamp);

    public interface ITechnicalSupportService
    {
        /// <summary>
        /// Sends a message to technical support.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendMessageAsync(string message);

        /// <summary>
        /// Retrieves the current chat history (placeholder).
        /// </summary>
        /// <returns>A list of chat messages.</returns>
        Task<List<ChatMessage>> GetChatHistoryAsync();
    }
}
