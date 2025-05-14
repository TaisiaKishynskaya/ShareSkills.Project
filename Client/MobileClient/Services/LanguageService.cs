using System;
using System.Globalization;
using Microsoft.Extensions.Localization;
using System.Threading;

namespace MobileClient.Services
{
    public interface ILanguageService
    {
        /// <summary>
        /// Gets or sets the current culture
        /// </summary>
        CultureInfo CurrentCulture { get; }

        /// <summary>
        /// Sets the application language
        /// </summary>
        /// <param name="cultureName">The culture name (e.g., "en", "uk")</param>
        Task SetLanguageAsync(string cultureName);

        /// <summary>
        /// Gets the current language code
        /// </summary>
        string GetCurrentLanguage();

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
        private readonly IPreferencesService _preferencesService;
        private const string LanguageKey = "language";
        private const string DefaultLanguage = "en";

        public CultureInfo CurrentCulture { get; private set; }

        // The available languages in the application
        private readonly List<LanguageInfo> _availableLanguages = new()
        {
            new LanguageInfo { Name = "English", DisplayName = "English", CultureCode = "en" },
            new LanguageInfo { Name = "Українська", DisplayName = "Українська", CultureCode = "uk" }
        };

        public LanguageService(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;

            // Initialize with default culture until InitializeLanguageAsync is called
            CurrentCulture = new CultureInfo(DefaultLanguage);
        }

        /// <summary>
        /// Initialize language settings from stored preferences
        /// </summary>
        public async Task InitializeLanguageAsync()
        {
            // Load the saved language or use default
            var savedLanguage = _preferencesService.Get(LanguageKey, DefaultLanguage);

            // Set culture using the loaded language code
            await SetLanguageAsync(savedLanguage);
        }

        /// <summary>
        /// Sets the application language
        /// </summary>
        public async Task SetLanguageAsync(string cultureName)
        {
            // Verify if the culture exists
            if (!_availableLanguages.Any(l => l.CultureCode == cultureName))
            {
                cultureName = DefaultLanguage; // Fall back to default if not supported
            }

            try
            {
                // Save the selected language
                _preferencesService.Set(LanguageKey, cultureName);

                // Update the current culture
                CurrentCulture = new CultureInfo(cultureName);

                // Set both CurrentCulture and CurrentUICulture
                CultureInfo.DefaultThreadCurrentCulture = CurrentCulture;
                CultureInfo.DefaultThreadCurrentUICulture = CurrentCulture;

                // Also set it on the current thread for immediate effect
                Thread.CurrentThread.CurrentCulture = CurrentCulture;
                Thread.CurrentThread.CurrentUICulture = CurrentCulture;

                // Ensure in a context that forces proper resource resolution
                await Task.Yield();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting culture: {ex.Message}");
                // Fall back to default language in case of error
                CurrentCulture = new CultureInfo(DefaultLanguage);
            }
        }

        /// <summary>
        /// Gets the current language code
        /// </summary>
        public string GetCurrentLanguage()
        {
            return _preferencesService.Get(LanguageKey, DefaultLanguage);
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