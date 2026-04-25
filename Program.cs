using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using student_profile.BLL;
using student_profile.BLL.Mapping;
using student_profile.BLL.Interfaces;
using student_profile.BLL.Repositories;
using student_profile.BLL.Services;
using student_profile.Data.Context;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DbContext configuration with Development fallback.
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var shouldUseInMemoryInDev =
    builder.Environment.IsDevelopment() &&
    (string.IsNullOrWhiteSpace(defaultConnection) ||
     defaultConnection.Contains("DESKTOP-IV6MTF1\\RUTHB", StringComparison.OrdinalIgnoreCase));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (shouldUseInMemoryInDev)
    {
        options.UseInMemoryDatabase("student_profile_dev");
        return;
    }

    options.UseSqlServer(defaultConnection);
});

// Repository registration
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPersonalDetailsRepository, PersonalDetailsRepository>();
builder.Services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IUserFileRepository, UserFileRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// Services registration
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<student_profile.BLL.IProjectService, ProjectService>();
builder.Services.AddScoped<student_profile.BLL.IFileService, FileService>();

// Additional BLL services from main branch
builder.Services.AddScoped<student_profile.BLL.IUserService, UserService>();
builder.Services.AddScoped<student_profile.BLL.IPersonalDetailsService, PersonalDetailsService>();
builder.Services.AddScoped<student_profile.BLL.IChatService, ChatService>();
builder.Services.AddScoped<student_profile.BLL.ISkillService, SkillService>();
builder.Services.AddScoped<student_profile.BLL.IPortfolioService, PortfolioService>();
builder.Services.AddScoped<student_profile.BLL.IImageService, ImageService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// JWT authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSection["Key"];
if (string.IsNullOrWhiteSpace(signingKey))
{
    throw new InvalidOperationException("JWT signing key is missing. Configure Jwt:Key in appsettings.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is ClaimsIdentity identity)
                {
                    var roleClaims = identity.FindAll("role")
                        .Concat(identity.FindAll("roles"))
                        .Where(c => !string.IsNullOrWhiteSpace(c.Value))
                        .Select(c => c.Value)
                        .Distinct(StringComparer.OrdinalIgnoreCase);

                    foreach (var role in roleClaims)
                    {
                        if (!identity.HasClaim(ClaimTypes.Role, role))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var bearerScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            Id = "Bearer"
        },
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Paste ONLY the JWT token (without 'Bearer ')."
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            bearerScheme,
            Array.Empty<string>()
        }
    });
});

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();