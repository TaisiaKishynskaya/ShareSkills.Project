namespace WebClient.Services
{
    public interface ICabinetService
    {
        public Task<User?> GetUser();
        Task<(bool Success, string ErrorMessage)> ChangeInfo(User userToChange, string newPassword);
    }
}
