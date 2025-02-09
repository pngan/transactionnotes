using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using transactionnotes.ApiService.Middleware;

var builder = WebApplication.CreateBuilder(args);


// Add authentication
// Specify in appsettings.Development.json for local development, or use environment variables in production
// E.g. TRANSNOTES__AUTHORITY, TRANSNOTES__CLIENTID, TRANSNOTES__CLIENTSECRET

// Specify the Keycloak realm URL, client ID, and client secret
var authority = builder.Configuration["TransNotes:Authority"];
var clientId = builder.Configuration["TransNotes:ClientId"];
var clientSecret = builder.Configuration["TransNotes:ClientSecret"];
var audience = builder.Configuration["TransNotes:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        //options.Audience = audience;
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuer = true,
            ValidIssuer = authority,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();


// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Use authentication & authorization
app.UseAuthentication();
app.UseAuthHeaderInspection();
app.UseAuthorization();


// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.RequireAuthorization();

app.MapDefaultEndpoints();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
