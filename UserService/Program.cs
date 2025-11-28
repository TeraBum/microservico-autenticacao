using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Claims;
using UserService.Data;
using UserService.Repositories;
using UserService.Services;
using UserService.Configurations;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

// Carrega .env (opcional)
DotEnv.Load();

// -----------------------------------------------------
// 🔥 1. Carrega JWT Settings a partir das environment vars
// -----------------------------------------------------
builder.Services.Configure<JwtSettings>(options =>
{
    options.SecretKey = Environment.GetEnvironmentVariable("JWTSETTINGS__SECRETKEY")
                      ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                      ?? throw new Exception("JWT secret não encontrado!");

    options.Issuer = Environment.GetEnvironmentVariable("JWTSETTINGS__ISSUER") ?? "UserService";
    options.Audience = Environment.GetEnvironmentVariable("JWTSETTINGS__AUDIENCE") ?? "UserClient";

    options.ExpirationMinutes = int.Parse(
        Environment.GetEnvironmentVariable("JWTSETTINGS__EXPIRATIONMINUTES") ?? "60"
    );
});

// -----------------------------------------------------
// 🔥 2. PEGA A CONNECTION STRING DIRETO DA ENV VARIABLE
// -----------------------------------------------------
var connectionString =
    Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION")
    ?? throw new Exception("CONNECTIONSTRINGS__DEFAULTCONNECTION NÃO encontrada!");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// -----------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -----------------------------------------------------
// 🔥 Swagger com JWT (apenas dev)
// -----------------------------------------------------
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Digite: Bearer {token}"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new string[] {}
            }
        });
    });
}

// -----------------------------------------------------
// 🔥 Autenticação JWT 100% garantida com env vars
// -----------------------------------------------------
var jwtSecret =
    Environment.GetEnvironmentVariable("JWTSETTINGS__SECRETKEY")
    ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new Exception("Nenhuma chave JWT encontrada!");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            RoleClaimType = ClaimTypes.Role
        };
    });

// -----------------------------------------------------
// Serviços
// -----------------------------------------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserServiceImpl>();
builder.Services.AddScoped<ITokenService, TokenService>();

// -----------------------------------------------------
// CORS
// -----------------------------------------------------
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000",
                "https://*.github.dev"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// -----------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
