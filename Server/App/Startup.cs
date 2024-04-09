using Libraries.Data;

namespace App;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
        => services.AddDbContext<AppDbContext>();

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}