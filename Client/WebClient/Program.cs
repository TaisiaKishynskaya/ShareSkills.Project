using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebClient.Components;
using WebClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
builder.Services.AddScoped<ICabinetService, CabinetService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IReportsService, ReportsService>();
builder.Services.AddScoped<ITechnicalSupportService, TechnicalSupportService>();

await builder.Build().RunAsync();
