using Microsoft.Extensions.Logging;

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

            return builder.Build();
        }
    }
}
