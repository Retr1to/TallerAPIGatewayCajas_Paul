using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ConfigureAppSettings(builder);
ConfigureServices(builder);

var app = builder.Build();

ConfigurePipeline(app);

app.Run();

static void ConfigureAppSettings(WebApplicationBuilder builder)
{
    // Agregar configuraciÃ³n de Ocelot
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    ConfigureSwagger(builder.Services);
    ConfigureJwt(builder);
    builder.Services.AddAuthorization();
    builder.Services.AddOcelot();
}

static void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "API Gateway",
            Version = "v1",
            Description = "API Gateway con autenticaciÃ³n JWT para gestiÃ³n de empleados"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Ingrese el token JWT en el formato: Bearer {token}"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });
}

static void ConfigureJwt(WebApplicationBuilder builder)
{
    var jwtKey = builder.Configuration["Jwt:Key"] ?? "MiClaveSecretaSuperSeguraParaJWT2024!";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "APIGateway";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "APIEmpleados";

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });
}

static void ConfigurePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseAuthentication();
    app.UseAuthorization();

    // Middleware para interceptar rutas de autenticaciÃ³n antes de Ocelot
    app.UseWhen(
        context => !context.Request.Path.StartsWithSegments("/api/auth"),
        appBuilder =>
        {
            appBuilder.UseOcelot().Wait();
        }
    );

    // Mapear controladores para /api/auth
    app.MapControllers();
}
