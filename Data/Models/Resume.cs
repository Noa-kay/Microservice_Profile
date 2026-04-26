namespace student_profile.Data.Models;

public enum ResumeTemplate
{
    ATSOptimized = 0,
    Executive = 1,
    Creative = 2,
    Minimal = 3
}

public class Resume
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? FileId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public ResumeTemplate Template { get; set; } = ResumeTemplate.ATSOptimized;
    public string Language { get; set; } = "en";
    public string Tone { get; set; } = "Professional";
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string ContentHtml { get; set; } = string.Empty;
    public string ContentPlainText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }

    public User User { get; set; } = null!;
    public UserFile? ExportedFile { get; set; }
}
