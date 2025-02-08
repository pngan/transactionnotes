using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using transactionnotes.Web;
using transactionnotes.Web.Components;
using transactionnotes.Web.Middleware;
using transactionnotes.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddTransient<AuthenticatedHttpClientHandler>();
builder.Services.AddTransient<DebuggingHttpHandler>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    })
    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
    .AddHttpMessageHandler<DebuggingHttpHandler>();


builder.Services.AddHttpContextAccessor();
// Specify in appsettings.Development.json for local development, or use environment variables in production
// E.g. TRANSNOTES__AUTHORITY, TRANSNOTES__CLIENTID, TRANSNOTES__CLIENTSECRET

// Specify the Keycloak realm URL, client ID, and client secret
var authority = builder.Configuration["TransNotes:Authority"];
var clientId = builder.Configuration["TransNotes:ClientId"];
var clientSecret = builder.Configuration["TransNotes:ClientSecret"];

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // auth service cannot use strict
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
}) // Use cookies for local authentication
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = authority;
    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.ResponseType = OpenIdConnectResponseType.Code; // Use Authorization Code Flow
    options.SaveTokens = true; // Save tokens in the authentication cookie
    options.GetClaimsFromUserInfoEndpoint = true; // Fetch additional claims from the user info endpoint
    options.Scope.Add("openid"); // Add required scopes
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.CallbackPath = "/signin-oidc"; // Callback path for the OIDC provider
    options.SignedOutCallbackPath = "/signout-callback-oidc"; // Callback path for sign-out
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// The following line that enables HTTPS redirection is commented out because the API service 
// should use plain HTTP when used behind a reverse proxy. We are deploying this application
// to a Docker compose environment where the reverse proxy will handle incoming external HTTPS
// connections, but uses plain HTTP to communicate internally with the API service.
//app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.Run();
