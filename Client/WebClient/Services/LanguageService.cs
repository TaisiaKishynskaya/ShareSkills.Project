using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace WebClient.Services
{
    public interface ILanguageService
    {
        /// <summary>
        /// Gets the current culture
        /// </summary>
        CultureInfo CurrentCulture { get; }

        /// <summary>
        /// Sets the application language
        /// </summary>
        /// <param name="cultureName">The culture name (e.g., "en", "uk")</param>
        /// <param name="reload">Whether to reload the page after changing the language</param>
        Task SetLanguageAsync(string cultureName, bool reload = true);

        /// <summary>
        /// Gets the current language code
        /// </summary>
        Task<string> GetCurrentLanguageAsync();

        /// <summary>
        /// Gets available languages
        /// </summary>
        IEnumerable<LanguageInfo> GetAvailableLanguages();

        /// <summary>
        /// Initializes the language from stored preferences
        /// </summary>
        Task InitializeLanguageAsync();
    }

    public class LanguageInfo
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string CultureCode { get; set; }
    }

    public class LanguageService : ILanguageService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string LanguageKey = "language";
        private const string DefaultLanguage = "en";

        public CultureInfo CurrentCulture { get; private set; }

        // The available languages in the application
        private readonly List<LanguageInfo> _availableLanguages = new()
        {
            new LanguageInfo { Name = "English", DisplayName = "English", CultureCode = "en" },
            new LanguageInfo { Name = "Українська", DisplayName = "Українська", CultureCode = "uk" }
        };

        public LanguageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;

            // Initialize with default culture until InitializeLanguageAsync is called
            CurrentCulture = new CultureInfo(DefaultLanguage);
        }

        /// <summary>
        /// Initialize language settings from browser storage
        /// </summary>
        public async Task InitializeLanguageAsync()
        {
            try
            {
                // Load the saved language or use default
                var savedLanguage = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", LanguageKey);

                if (string.IsNullOrEmpty(savedLanguage))
                {
                    // No saved language, use default
                    savedLanguage = DefaultLanguage;
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", LanguageKey, savedLanguage);
                }

                // Set culture using the loaded language code WITHOUT triggering a reload
                await SetLanguageAsync(savedLanguage, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing language: {ex.Message}");
                // Fall back to default language
                await SetLanguageAsync(DefaultLanguage, false);
            }
        }

        /// <summary>
        /// Sets the application language
        /// </summary>
        /// <param name="cultureName">Culture code to set</param>
        /// <param name="reload">Whether to reload the page</param>
        public async Task SetLanguageAsync(string cultureName, bool reload = true)
        {
            Console.WriteLine("language" + cultureName);
            // Verify if the culture exists
            if (!_availableLanguages.Any(l => l.CultureCode == cultureName))
            {
                cultureName = DefaultLanguage; // Fall back to default if not supported
            }

            try
            {
                // Save the selected language
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", LanguageKey, cultureName);

                // Update the current culture
                CurrentCulture = new CultureInfo(cultureName);

                // Set both CurrentCulture and CurrentUICulture on the current thread
                CultureInfo.DefaultThreadCurrentCulture = CurrentCulture;
                CultureInfo.DefaultThreadCurrentUICulture = CurrentCulture;

                // Only reload if explicitly requested
                if (reload)
                {
                    await _jsRuntime.InvokeVoidAsync("location.reload");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting culture: {ex.Message}");
                // Fall back to default language in case of error
                CurrentCulture = new CultureInfo(DefaultLanguage);
            }
        }

        /// <summary>
        /// Gets the current language code
        /// </summary>
        public async Task<string> GetCurrentLanguageAsync()
        {
            var language = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", LanguageKey);
            return string.IsNullOrEmpty(language) ? DefaultLanguage : language;
        }

        /// <summary>
        /// Gets available languages
        /// </summary>
        public IEnumerable<LanguageInfo> GetAvailableLanguages()
        {
            return _availableLanguages;
        }
    }
}