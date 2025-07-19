using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using central.api.Middleware;

//using transactionnotes.ApiService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


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


        // This allows you to hook into the JWT validation process.
        // This is where you'll create or update your internal Identity user
        // and enrich the ClaimsPrincipal with your internal roles.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                //var subject = context.HttpContext.Items[HttpContextItems.UserJwtSub] as string;

                //  var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();

                //// Get the unique identifier from the Keycloak token
                //// Keycloak typically uses 'sub' (subject) as the unique user ID
                //var keycloakUserId = context.Principal?.FindFirst("sub")?.Value;
                //var keycloakUsername = context.Principal?.FindFirst("preferred_username")?.Value ?? context.Principal?.Identity?.Name; // Get a display name

                //if (string.IsNullOrEmpty(keycloakUserId))
                //{
                //    context.Fail("Missing 'sub' claim from Keycloak token.");
                //    return;
                //}


                /*
                // 1. Find or create the internal Identity user
                var identityUser = await userManager.FindByIdAsync(keycloakUserId); // Try to find by external ID
                if (identityUser == null)
                {
                    // If not found, create a new Identity user.
                    // You might need to map more claims from Keycloak (e.g., email)
                    identityUser = new IdentityUser
                    {
                        Id = keycloakUserId, // Use Keycloak's 'sub' as our internal Identity ID
                        UserName = keycloakUsername, // Use preferred_username or similar
                        Email = context.Principal?.FindFirst("email")?.Value,
                        EmailConfirmed = true // Assume confirmed if authenticated by Keycloak
                    };
                    var createResult = await userManager.CreateAsync(identityUser);
                    if (!createResult.Succeeded)
                    {
                        context.Fail($"Failed to create internal user: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                        return;
                    }
                    // Optional: Store the original Keycloak username if 'UserName' changes
                    // await userManager.AddClaimAsync(identityUser, new Claim("KeycloakUsername", keycloakUsername));
                }
                else
                {
                    // User already exists, update properties if necessary (e.g., email, username)
                    bool changed = false;
                    if (identityUser.UserName != keycloakUsername)
                    {
                        identityUser.UserName = keycloakUsername;
                        changed = true;
                    }
                    var keycloakEmail = context.Principal?.FindFirst("email")?.Value;
                    if (identityUser.Email != keycloakEmail)
                    {
                        identityUser.Email = keycloakEmail;
                        changed = true;
                    }
                    if (changed)
                    {
                        await userManager.UpdateAsync(identityUser);
                    }
                }

                // 2. Load internal Identity roles and claims for the user
                var internalClaims = await userManager.GetClaimsAsync(identityUser);
                var internalRoles = await userManager.GetRolesAsync(identityUser);
                */
                // Construct a new ClaimsIdentity with *only* your internal roles/claims
                var newIdentity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);


                //// Add the essential claims from the Keycloak token (e.g., sub, name, preferred_username, email)
                //foreach (var claim in context.Principal.Claims)
                //{
                //    // Filter out Keycloak's role claims if they exist, or just add all standard ones
                //    if (claim.Type != "realm_access" && claim.Type != "resource_access" && claim.Type != ClaimTypes.Role)
                //    {
                //        newIdentity.AddClaim(claim);
                //    }
                //}

                //// Add our internal roles as claims
                //foreach (var role in internalRoles)
                //{
                //    newIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                //}

                //// Add our internal custom claims
                //foreach (var claim in internalClaims)
                //{
                //    newIdentity.AddClaim(claim);
                //}

                // Add CanWrite claim if the user has the transactionnotes-write role
                var identity = context.Principal?.Identity as ClaimsIdentity;
                identity.AddClaim(new Claim("Permission", "WritePermission"));


                // Set the new ClaimsPrincipal for the current request
                // context.Principal = new ClaimsPrincipal(newIdentity);

                return Task.CompletedTask;



                // If you want to use Identity's SignInManager for creating the session cookie
                // (useful for traditional Razor Pages/MVC or if you need to mimic a login state)
                // await signInManager.SignInAsync(identityUser, isPersistent: true, context.Principal.Claims.ToArray());
                // Note: For pure API scenarios, the above signInManager line is often not needed,
                // as the JWT itself authenticates each request. For Blazor Server, it might be.
            },
            OnAuthenticationFailed = context =>
            {
                // Log authentication failures for debugging
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "JWT authentication failed.");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {

                return Task.CompletedTask;
            }
        };


    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


// Use authentication & authorization
app.UseAuthentication();
app.UseAuthHeaderInspection();
app.UseAuthorization();

app.MapControllers();

app.Run();
