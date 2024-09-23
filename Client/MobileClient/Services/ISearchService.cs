using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Services
{
    public interface ISearchService
    {
        public Task<Teacher?> SearchTeacher(string skill, string time, string level);
        public Task<Teacher?> GetTeacherById(string id);
        public Task<List<Teacher>?> GetTeachers();
        public Task<Teacher?> GetTeacherByEmail(string email);
    }
}
