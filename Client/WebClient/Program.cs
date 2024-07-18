using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebClient.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CalendarService>();
builder.Services.AddScoped<CabinetService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<SearchService>();

await builder.Build().RunAsync();
