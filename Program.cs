using AutoMapper;
using MarketingNotificationService.Data.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using student_profile.BLL;
using student_profile.BLL.Interfaces;
using student_profile.BLL.Repositories;
using student_profile.BLL.Services;
using student_profile.BLL.Mapping;
using student_profile.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// *** FIXED: Added DbContext configuration ***
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddScoped<student_profile.BLL.IProjectService, student_profile.BLL.ProjectService>();
builder.Services.AddScoped<student_profile.BLL.IFileService, student_profile.BLL.FileService>();

// Additional BLL services from main branch
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPersonalDetailsService, PersonalDetailsService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IImageService, ImageService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services to the container.
var serviceSettings = builder.Configuration
    .GetSection("ServiceSettings")
    .Get<ServiceSettings>();
var rabbitMQSettings = builder.Configuration
    .GetSection("RabbitMQSettings")
    .Get<RabbitMQSettings>();

builder.Services.AddControllers();

builder.Services.AddMassTransit(configure =>
{
    var rabbitSection = builder.Configuration.GetSection("RabbitMq");
    var host = rabbitSection["Host"] ?? rabbitMQSettings?.Host ?? "localhost";
    var username = rabbitSection["Username"] ?? "guest";
    var password = rabbitSection["Password"] ?? "guest";
    var graduatesEntityName = rabbitSection["GraduatesMessageEntityName"] ?? "graduates-count";

    // ב-Publisher בדרך כלל אין צרכנים
    configure.AddConsumers(Assembly.GetEntryAssembly());

    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        configurator.Message<graduates_count>(x => x.SetEntityName(graduatesEntityName));

        // הגדרת פורמט השמות (Kebab Case)
        configurator.ConfigureEndpoints(context, 
            new KebabCaseEndpointNameFormatter(
                serviceSettings?.ServiceName ?? "student-profile-service", false));

        // הגדרת ניסיונות חוזרים (Retry Policy)
        configurator.UseMessageRetry(retryConfigurator =>
        {
            retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
        });
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
app.UseAuthorization();
app.MapControllers();

app.Run();