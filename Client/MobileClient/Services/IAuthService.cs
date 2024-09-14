﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Services
{
    public interface IAuthService
    {
        public Task<bool> UserLogin(string email, string password);
        public Task<bool> Register(bool IsTeacher, string Name, string Surname, String Email, string Password);
        public Task getUserRole();
        public Task<List<Skill>?> GetSkills();
        public Task<bool> ChangeSkills(string id, string skill, string time, string level);
    }
}