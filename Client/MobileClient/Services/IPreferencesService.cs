using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Services
{
    public interface IPreferencesService
    {
        string? Get(string key, string defaultValue);
        void Set(string key, string value);

        void Clear();
    }
}
