using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;
using ProfilMicroservice.Mappers;
using student_profile.Data.Context;
using student_profile.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPersonalDetailsRepository, PersonalDetailsRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();

// AutoMapper (for your DTOs + aggregates)
builder.Services.AddAutoMapper(typeof(UserMapperProfile).Assembly);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
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

// HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Existing minimal API endpoint using your aggregate + AutoMapper
app.MapGet("/api/profile/{userId:guid}", (Guid userId, IMapper mapper) =>
{
    var aggregate = new UserProfileAggregate
    {
        User = new User { Id = userId },
        PersonalDetails = new PersonalDetails
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Demo User",
            Email = "demo@example.com",
            Phone = "050-0000000",
            Address = "Demo Address",
            Bio = "Demo bio"
        },
        Projects = new[]
        {
            new Project
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = "Demo Project",
                Description = "Demo description",
                ProjectName = "Demo",
                GitHubLink = "https://github.com/demo/demo"
            }
        },
        Resumes = Array.Empty<UserFile>(),
        UserSkills = Array.Empty<SkillToUser>(),
        SkillCategories = Array.Empty<Category>(),
        ChatHistory = null
    };

    var dto = mapper.Map<UserProfileDto>(aggregate);
    return Results.Ok(dto);
})
.WithName("GetUserProfile")
.WithOpenApi();

app.Run();
