using Microsoft.Extensions.Logging;
using MobileClient.Services;
using System.Net;

namespace MobileClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer, // Управляем куки
                UseCookies = true,
                AllowAutoRedirect = true
            };

            // Регистрируем HttpClientHandler как singleton, чтобы использовать один экземпляр
            builder.Services.AddSingleton(handler);

            // Регистрируем HttpClient, используя тот же handler
            builder.Services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5115/");
            }).ConfigurePrimaryHttpMessageHandler(() => handler);

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources/Strings");
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<CalendarService>();
            builder.Services.AddScoped<CabinetService>();
            builder.Services.AddScoped<SearchService>();
            builder.Services.AddScoped<FeedbackService>();
            builder.Services.AddScoped<IPreferencesService, PreferencesService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICabinetService, CabinetService>();
            builder.Services.AddScoped<ICalendarService, CalendarService>();
            builder.Services.AddScoped<ISearchService, SearchService>();

            return builder.Build();
        }
    }
}
