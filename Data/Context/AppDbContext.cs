using Microsoft.EntityFrameworkCore;
using student_profile.Data.Models;

namespace student_profile.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<SkillToUser> SkillToUsers { get; set; } = null!;
    public DbSet<ChatHistory> ChatHistories { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<student_profile.Data.Models.UserFile> Files { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SkillToUser composite key
        modelBuilder.Entity<SkillToUser>()
            .HasKey(stu => new { stu.UserId, stu.SkillId });

        // Project: UserId → User (One-to-Many)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Skill: CategoryId → Category (Many-to-One)
        modelBuilder.Entity<Skill>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Skills)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // SkillToUser: UserId → User
        modelBuilder.Entity<SkillToUser>()
            .HasOne(stu => stu.User)
            .WithMany(u => u.SkillToUsers)
            .HasForeignKey(stu => stu.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // SkillToUser: SkillId → Skill
        modelBuilder.Entity<SkillToUser>()
            .HasOne(stu => stu.Skill)
            .WithMany(s => s.SkillToUsers)
            .HasForeignKey(stu => stu.SkillId)
            .OnDelete(DeleteBehavior.NoAction);

        // ChatHistory: UserId → User
        modelBuilder.Entity<ChatHistory>()
            .HasOne(ch => ch.User)
            .WithMany(u => u.ChatHistories)
            .HasForeignKey(ch => ch.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message: SenderId → User
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.Messages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Message: ChatHistoryId → ChatHistory
        modelBuilder.Entity<Message>()
            .HasOne(m => m.ChatHistory)
            .WithMany(ch => ch.Messages)
            .HasForeignKey(m => m.ChatHistoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // File: UserId → User
        modelBuilder.Entity<student_profile.Data.Models.UserFile>()
            .HasOne(f => f.User)
            .WithMany(u => u.Files)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}