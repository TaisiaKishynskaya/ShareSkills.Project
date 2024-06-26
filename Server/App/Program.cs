using App.Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------------------------------------------------
// Удалите этот код, если правильно пропишите код в конфигурациях Арр
// СМОТРЕТЬ https://github.com/TaisiaKishynskaya/CSharp_A-Level/tree/main/eShop.Project/Application/Catalog/Catalog.API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------------------------------------------------------------------------------------------
// ДОБАВЬТЕ ЭТОТ КОД когда пропишете классы с конфигурациями

// AuthenticationConfiguration.ConfigureAuthentication(builder);
// AuthorizationConfiguration.ConfigureAuthorization(builder); 

// SwaggerConfiguration.AddSwagger(builder.Services, builder.Configuration); 

 DatabaseConfiguration.ConfigureDatabase(builder);

// ServicesConfiguration.ConfigureServices(builder); 

//----------------------------------------------------------------------------------------------------------------
var app = builder.Build();

// Когда пропишете все файлы конфигураций - удалите строки ниже (кроме "AppConfiguration.ConfigureApp(app);"  и  "app.Run();")

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// AppConfiguration.ConfigureApp(app);

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () => { })
    .WithName("GetWeatherForecast")
    .WithOpenApi();


app.Run();
