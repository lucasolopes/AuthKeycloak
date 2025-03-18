using System.Security.Claims;
using AuthKeycloak.Extensions;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGenWithAuth(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization(options =>
{
    // Política para usuários com role "api_user"
    options.AddPolicy("ApiUser", policy =>
        policy.RequireRole("api_user"));

    // Política para usuários com role "api_admin"
    options.AddPolicy("ApiAdmin", policy =>
        policy.RequireRole("api_admin"));

    // Política combinando múltiplas roles com OR (usuário precisa ter pelo menos uma das roles)
    options.AddPolicy("ApiManagerOrAdmin", policy =>
        policy.RequireRole("api_manager", "api_admin"));

    // Política baseada em claims personalizados
    options.AddPolicy("CanWriteData", policy =>
        policy.RequireClaim("permissions", "write"));
        
    // Política baseada em atributos de role
    options.AddPolicy("HasAdminPermissions", policy =>
        policy.RequireAssertion(context => 
            context.User.Claims.Any(c => 
                c.Type.Contains("attributes") && 
                c.Value.Contains("permissions") && 
                c.Value.Contains("admin")
            )
        ));
});

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, o =>
{
    o.RequireHttpsMetadata = false;
    o.Audience = builder.Configuration["Authentication:Audience"];
    o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
    };
});

builder.Services.AddHttpClient<TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("user/me", (ClaimsPrincipal claimsPrincipal) =>
{
    return claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value);
}).RequireAuthorization();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
