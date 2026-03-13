using AutoMapper;
using ProfilMicroservice.Domain;
using ProfilMicroservice.DTOs;
using ProfilMicroservice.Mappers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(UserMapperProfile).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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
                GitHubLink = "https://github.com/demo/demo",
                ProjectsImages = new[] { "https://example.com/image1.png" }
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
