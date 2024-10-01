using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Services
{
    public interface ICalendarService
    {
        public Task<List<Meeting>?> UpdateCalendar(DateTime startDate, DateTime endDate);
        public Task<string?> GetIdByEmail(string email);
        public Task<bool> AddMeeting(DateTime Date, string Email, String Title);
        public Task<(Meeting meeting, User teacher)?> GetMeetingInfo(string Id, String userRole);
    }
}
