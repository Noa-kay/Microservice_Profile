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
    public DbSet<Image> Images { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<SkillToUser> SkillToUsers { get; set; } = null!;
    public DbSet<ChatHistory> ChatHistories { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<UserFile> Files { get; set; } = null!;
    public DbSet<PersonalDetails> PersonalDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SkillToUser composite key
        modelBuilder.Entity<SkillToUser>()
            .HasKey(stu => new { stu.UserId, stu.SkillId });

        // User → Projects (1:N)
        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Project → Images (1:N)
        modelBuilder.Entity<Image>()
            .HasOne(i => i.Project)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Category → Skills (1:N)
        modelBuilder.Entity<Skill>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Skills)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // SkillToUser → User (N:N bridge)
        modelBuilder.Entity<SkillToUser>()
            .HasOne(stu => stu.User)
            .WithMany(u => u.SkillToUsers)
            .HasForeignKey(stu => stu.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // SkillToUser → Skill
        modelBuilder.Entity<SkillToUser>()
            .HasOne(stu => stu.Skill)
            .WithMany(s => s.SkillToUsers)
            .HasForeignKey(stu => stu.SkillId)
            .OnDelete(DeleteBehavior.NoAction);

        // User ↔ ChatHistory (1:1)
        modelBuilder.Entity<ChatHistory>()
            .HasOne(ch => ch.User)
            .WithOne(u => u.ChatHistory)
            .HasForeignKey<ChatHistory>(ch => ch.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ChatHistory → Messages (1:N)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.ChatHistory)
            .WithMany(ch => ch.Messages)
            .HasForeignKey(m => m.ChatHistoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message → Sender (User)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // User → Files (1:N)
        modelBuilder.Entity<UserFile>()
            .HasOne(f => f.User)
            .WithMany(u => u.Files)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User ↔ PersonalDetails (1:1)
        modelBuilder.Entity<PersonalDetails>()
            .HasOne<User>()
            .WithOne(u => u.PersonalDetails)
            .HasForeignKey<PersonalDetails>(pd => pd.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}