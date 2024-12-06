namespace WebClient.Services
{
    public interface IAuthService
    {
        public Task<ValidationResponse> UserLogin(string email, string password);

        public Task<ValidationResponse> Register(bool IsTeacher, string Name, string Surname, String Email,
            string Password);

        public Task GetUserRole();
        public Task<List<Skill>?> GetSkills();
        public Task<bool> ChangeSkills(string id, string skill, string time, string level);
        public Task AllowCookies();
        public Task DenyCookies();
        public Task<string> GetCookiesPermission();
        public Task<bool> GetCookies();
    }
}
