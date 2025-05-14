using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebClient.Components;
using WebClient.Services;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using WebClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources/Strings");
builder.Services.AddSingleton<ISharedResource, SharedResource>();
builder.Services.AddScoped<ILanguageService, LanguageService>();

// Other services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<ICabinetService, CabinetService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<ITechnicalSupportService, TechnicalSupportService>();

// Set default culture 
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en");

var host = builder.Build();

// Configure language from storage if available
var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
var languageService = host.Services.GetRequiredService<ILanguageService>();

// Initialize with reload=false to prevent infinite reloads
await languageService.InitializeLanguageAsync();

await host.RunAsync();