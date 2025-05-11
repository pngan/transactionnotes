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

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

// Add ValidateKeycloakJWTMiddleware to the application
app.AddUserMiddleware();

// Use authentication & authorization
app.UseAuthentication();
app.UseAuthHeaderInspection();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapDefaultEndpoints();
app.Run();
