using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Services
{
    public interface IFeedbackService
    {
        public Task<bool> SendFeedback(string email, int grade);
    }
}
