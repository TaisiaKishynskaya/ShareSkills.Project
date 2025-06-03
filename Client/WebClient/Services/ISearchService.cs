namespace WebClient.Services
{
    public interface ISearchService
    {
        public Task<Teacher?> SearchTeacher(string skill, string time, string level);
        public Task<Teacher?> GetTeacherById(string id);
        public Task<List<Teacher>?> GetTeachers(string userId);
        public Task<Teacher?> GetTeacherByEmail(string email);
        
        public Task<Course?> SearchCourse(string skill, string time, string level);
        public Task<Course?> GetCourseById(string id);
        public Task<List<Course>?> GetCourses(string userId);
    }
}
