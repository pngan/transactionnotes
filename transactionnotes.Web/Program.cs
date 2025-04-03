using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
var audience = builder.Configuration["TransNotes:Audience"];

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
    options.TokenValidationParameters.ValidAudience = audience;
    options.ResponseType = OpenIdConnectResponseType.Code; // Use Authorization Code Flow
    options.SaveTokens = true; // Save tokens in the authentication cookie
    //options.GetClaimsFromUserInfoEndpoint = true; // Fetch additional claims from the user info endpoint
    options.Scope.Add("openid"); // Add required scopes
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.CallbackPath = "/signin-oidc"; // Callback path for the OIDC provider
    options.SignedOutCallbackPath = "/signout-callback-oidc"; // Callback path for sign-out
    options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme; // Use the OIDC scheme for sign-out
    options.Events.OnTokenValidated = async context =>
    {
        string token = context.TokenEndpointResponse.AccessToken;
        var handler = new JwtSecurityTokenHandler();
        var parsedJwt = handler.ReadJwtToken(token);

        // For some reason, this is not enough.
        // The `role` claim is just being set to "role" as the claim type.
        // But Microsoft requires using their enum, `ClaimTypes.Role` if you want to use the claims with the `[Authorize(Roles = "...")]` Annotation.
        // So, we need to convert any "role" claims in the JWT to the actual Microsoft enum for them to be properly picked up...
        // So convert them here I guess...
        var updatedClaims = parsedJwt.Claims.ToList().Select(c =>
        {
            return c.Type == "role" ? new Claim(ClaimTypes.Role, c.Value) : c;
        });


        // Finally, use the new claims list and add a new `Identity` that contains them.
        context.Principal.AddIdentity(new ClaimsIdentity(updatedClaims));
    };
})
;
builder.Services.AddScoped<ErrorHandlingService>();

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

app.MapPost("/account/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
});
app.MapPost("/account/login", async (HttpContext context) =>
{
    var redirectUri = "/";
    var properties = new AuthenticationProperties { RedirectUri = redirectUri };

    if ((context?.User?.Identity == null) || !context.User.Identity.IsAuthenticated)
    {
        await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, properties);
    }
});

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.Run();
