using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Keycloak.Poc;
using Keycloak.Poc.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin() // Allow any origin (all hosts)
            .AllowAnyMethod() // Allow any HTTP method (GET, POST, etc.)
            .AllowAnyHeader(); // Allow any header
    });
});

IdentityModelEventSource.ShowPII = true;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

    })
    .AddCookie()
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        try
        {
            //todo: change to localhost locally
            if (builder.Environment.IsDevelopment())
            {
                // Local development settings
                //options.Authority = "http://keycloak:8080/realms/YAMS";
                //options.Authority = "http://host.docker.internal:8081/realms/YAMS";
                options.Authority = "http://ngrp/realms/YAMS";
                 //options.Authority = "http://localhost:9010/realms/YAMS";
                //options.Authority = "http://ngrp/realms/YAMS";

            }
            else
            {
                // Podman container settings, ngnix url 
                options.Authority = "http://keycloak:8080/realms/YAMS";
            }

            //TODO: read this from the config
            options.ClientId = "yams-client";
            options.ClientSecret = "myvUpGC7HFu0i6uPdZeSqkIl4chDjp1X";
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            options.TokenValidationParameters.RoleClaimType = "role";
            options.ClaimActions.MapJsonKey("role", "role");

            options.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = MapRoles,
                OnRedirectToIdentityProvider = context =>
                {
                    // Override the authority for redirect URIs going to the browser
                    context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress
                        .Replace("http://ngrp", "http://localhost:8080");

                    return Task.CompletedTask;
                }
            };
        }
        catch (Exception ex)
        {
        }

    });



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("all", policy => policy.RequireRole("all"));
    options.AddPolicy("licence", policy => policy.RequireRole("licence"));
    options.AddPolicy("install", policy => policy.RequireRole("install"));
    options.AddPolicy("info", policy => policy.RequireRole("info"));
 
});
builder.Services.AddMvc();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Logging.AddConsole();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();
app.UseCors("AllowAllOrigins");

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts();
}
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();



Task MapRoles(TokenValidatedContext tokenValidatedContext)
{
    var claimsIdentity = (ClaimsIdentity)tokenValidatedContext.Principal?.Identity!;
    var roleClaims = tokenValidatedContext.Principal?.FindAll("role");
    if (roleClaims is null) return Task.CompletedTask;
    foreach (var roleClaim in roleClaims)
    {
        var roles = roleClaim.Value.Split(',');
        foreach (var role in roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }
    var idToken = tokenValidatedContext.SecurityToken.RawData;

    claimsIdentity.AddClaim(new Claim("id_token", idToken));
    // Logging for debugging purposes
    var logger = tokenValidatedContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
    var rolesInPrincipal = tokenValidatedContext.Principal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
    logger.LogInformation("Roles mapped: {Roles}", string.Join(", ", rolesInPrincipal));

    return Task.CompletedTask;
}