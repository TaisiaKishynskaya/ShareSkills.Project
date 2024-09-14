using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileClient.Services
{
    public class PreferencesService : IPreferencesService
    {
        public string? Get(string key, string defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }

        public void Set(string key, string value) { Preferences.Set(key, value); }

        public void Clear()
        {
            Preferences.Clear();
        }
    }
}
