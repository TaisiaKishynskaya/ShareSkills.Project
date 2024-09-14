using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Services
{
    public interface ICabinetService
    {
        public Task<User?> GetUser();
        public Task<bool> ChangeInfo(User userToChange, string newPassword);

    }
}
