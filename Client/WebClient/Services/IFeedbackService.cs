namespace WebClient.Services
{
    public interface IFeedbackService
    {
        public Task<bool> SendFeedback(string email, int grade);
    }
}
