using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using FormEngineAPI.Data;
using FormEngineAPI.Repositories;
using FormEngineAPI.Services;
using FormEngineAPI.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, mySqlOptions =>
    {
        // Adiciona retry em caso de falha transitória
        mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFormRepository, FormRepository>();
builder.Services.AddScoped<IFormSubmissionRepository, FormSubmissionRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFormService, FormService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IMenuService, MenuService>();

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new Exception("JWT SecretKey não configurada");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FormEngine API",
        Version = "v1",
        Description = "API REST para gerenciamento de formulários dinâmicos com FormEngine",
        Contact = new OpenApiContact
        {
            Name = "FormEngine Team",
            Email = "contact@formengine.com"
        }
    });

    // Configurar JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    // Aguarda o banco de dados ficar disponível e aplica as migrations
    await WaitForDatabaseAsync(services, logger);
}

async Task WaitForDatabaseAsync(IServiceProvider services, ILogger logger)
{
    const int maxAttempts = 60; // 60 tentativas (5 minutos)
    const int delayMs = 5000;   // 5 segundos entre tentativas
    
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            logger.LogInformation("Tentativa {Attempt}/{MaxAttempts} - Verificando conexão com o banco de dados...", attempt, maxAttempts);
            
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // Testa a conexão
            await context.Database.CanConnectAsync();
            logger.LogInformation("✓ Conexão com banco de dados estabelecida!");
            
            // Aplica as migrations
            logger.LogInformation("Aplicando migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("✓ Migrations aplicadas com sucesso!");
            
            return; // Sucesso - sai do loop
        }
        catch (Exception ex)
        {
            logger.LogWarning("Tentativa {Attempt} falhou: {Error}", attempt, ex.Message);
            
            if (attempt == maxAttempts)
            {
                logger.LogError(ex, "❌ Falha ao conectar com o banco de dados após {MaxAttempts} tentativas", maxAttempts);
                throw; // Re-lança a exceção na última tentativa
            }
            
            logger.LogInformation("Aguardando {Delay}ms antes da próxima tentativa...", delayMs);
            await Task.Delay(delayMs);
        }
    }
}

// Configure the HTTP request pipeline
// Habilitar Swagger em todos os ambientes (incluindo Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FormEngine API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
