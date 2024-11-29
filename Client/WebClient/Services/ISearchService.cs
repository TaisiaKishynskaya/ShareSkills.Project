namespace WebClient.Services
{
    public interface ISearchService
    {
        public Task<Teacher?> SearchTeacher(string skill, string time, string level);
        public Task<Teacher?> GetTeacherById(string id);
        public Task<List<Teacher>?> GetTeachers();
        public Task<Teacher?> GetTeacherByEmail(string email);
    }
}
