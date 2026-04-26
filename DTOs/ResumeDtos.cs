using student_profile.Data.Models;

namespace student_profile.DTOs;

public class GenerateResumeRequestDto
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public ResumeTemplate Template { get; set; } = ResumeTemplate.ATSOptimized;
    public string Language { get; set; } = "en";
    public string Tone { get; set; } = "Professional";
    public bool SetAsActive { get; set; } = true;
}

public class ResumeDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TargetRole { get; set; } = string.Empty;
    public ResumeTemplate Template { get; set; }
    public string Language { get; set; } = "en";
    public string Tone { get; set; } = "Professional";
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public string ContentHtml { get; set; } = string.Empty;
    public string ContentPlainText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}
