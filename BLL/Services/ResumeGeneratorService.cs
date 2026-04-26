using System.Net;
using Microsoft.EntityFrameworkCore;
using student_profile.BLL.Interfaces;
using student_profile.Data.Context;
using student_profile.Data.Models;
using student_profile.Data.Repositories;

namespace student_profile.BLL.Services;

public class ResumeGeneratorService : IResumeGeneratorService
{
    private readonly AppDbContext _context;
    private readonly IResumeRepository _resumeRepository;

    public ResumeGeneratorService(AppDbContext context, IResumeRepository resumeRepository)
    {
        _context = context;
        _resumeRepository = resumeRepository;
    }

    public async Task<Resume> GenerateResumeAsync(
        Guid userId,
        string title,
        string targetRole,
        ResumeTemplate template,
        string language,
        string tone,
        bool setAsActive,
        CancellationToken ct = default)
    {
        var details = await _context.PersonalDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (details is null)
        {
            throw new InvalidOperationException("Personal details were not found for this user.");
        }

        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.Title)
            .Take(6)
            .ToListAsync(ct);

        var skills = await _context.SkillToUsers
            .AsNoTracking()
            .Include(s => s.Skill)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.YearsOfExperience)
            .ThenBy(s => s.Skill.Name)
            .Take(16)
            .ToListAsync(ct);

        var normalizedLanguage = NormalizeLanguage(language);
        var normalizedTone = NormalizeTone(tone);

        var plainText = BuildPlainText(details, projects, skills, targetRole, normalizedLanguage, normalizedTone);
        var html = BuildHtml(details, projects, skills, title, targetRole, template, normalizedLanguage, normalizedTone);

        var nextVersion = await _context.Resumes
            .Where(r => r.UserId == userId && r.TargetRole == targetRole)
            .Select(r => r.Version)
            .DefaultIfEmpty(0)
            .MaxAsync(ct) + 1;

        var resume = new Resume
        {
            UserId = userId,
            Title = string.IsNullOrWhiteSpace(title) ? $"{details.Name} - Resume" : title.Trim(),
            TargetRole = targetRole.Trim(),
            Template = template,
            Language = normalizedLanguage,
            Tone = normalizedTone,
            Version = nextVersion,
            IsActive = setAsActive,
            ContentHtml = html,
            ContentPlainText = plainText
        };

        var created = await _resumeRepository.CreateAsync(resume, ct);

        if (setAsActive)
        {
            await _resumeRepository.SetActiveAsync(created.Id, userId, ct);
        }

        return created;
    }

    private static string NormalizeLanguage(string language)
    {
        var value = (language ?? string.Empty).Trim().ToLowerInvariant();
        return value == "he" ? "he" : "en";
    }

    private static string NormalizeTone(string tone)
    {
        if (string.IsNullOrWhiteSpace(tone))
        {
            return "Professional";
        }

        var normalized = tone.Trim();
        return normalized.ToLowerInvariant() switch
        {
            "creative" => "Creative",
            "casual" => "Casual",
            _ => "Professional"
        };
    }

    private static string BuildPlainText(
        PersonalDetails details,
        IReadOnlyCollection<Project> projects,
        IReadOnlyCollection<SkillToUser> skills,
        string targetRole,
        string language,
        string tone)
    {
        var lines = new List<string>
        {
            details.Name,
            $"{details.Email} | {details.Phone} | {details.Address}",
            string.Empty,
            language == "he" ? $"תפקיד יעד: {targetRole}" : $"Target Role: {targetRole}",
            language == "he" ? $"סגנון: {tone}" : $"Tone: {tone}",
            string.Empty,
            language == "he" ? "תקציר מקצועי" : "Professional Summary",
            BuildSummary(details.Bio, targetRole, language, tone),
            string.Empty,
            language == "he" ? "כישורים מרכזיים" : "Core Skills"
        };

        lines.AddRange(skills.Select(s => $"- {s.Skill.Name} ({s.YearsOfExperience}y)"));

        lines.Add(string.Empty);
        lines.Add(language == "he" ? "פרויקטים נבחרים" : "Selected Projects");
        lines.AddRange(projects.Select(p =>
            $"- {p.Title}: {TrimSentence(p.Description, language)}{BuildGitHubSuffix(p.GitHubLink, language)}"));

        return string.Join(Environment.NewLine, lines);
    }

    private static string BuildHtml(
        PersonalDetails details,
        IReadOnlyCollection<Project> projects,
        IReadOnlyCollection<SkillToUser> skills,
        string title,
        string targetRole,
        ResumeTemplate template,
        string language,
        string tone)
    {
        var encodedName = Encode(details.Name);
        var encodedEmail = Encode(details.Email);
        var encodedPhone = Encode(details.Phone);
        var encodedAddress = Encode(details.Address);
        var encodedTitle = Encode(title);
        var encodedTargetRole = Encode(targetRole);

        var summary = Encode(BuildSummary(details.Bio, targetRole, language, tone));
        var direction = language == "he" ? "rtl" : "ltr";
        var font = template == ResumeTemplate.Creative ? "'Trebuchet MS', sans-serif" : "Arial, sans-serif";
        var accent = template switch
        {
            ResumeTemplate.Executive => "#1f2937",
            ResumeTemplate.Creative => "#6d28d9",
            ResumeTemplate.ATSOptimized => "#111827",
            _ => "#0f766e"
        };

        var skillsHtml = string.Join("", skills.Select(s =>
            $"<li><strong>{Encode(s.Skill.Name)}</strong> - {s.YearsOfExperience}y</li>"));

        var projectsHtml = string.Join("", projects.Select(p =>
            $"<article class='item'><h3>{Encode(p.Title)}</h3><p>{Encode(TrimSentence(p.Description, language))}</p>{BuildGitHubAnchor(p.GitHubLink, language)}</article>"));

        var summaryLabel = language == "he" ? "תקציר מקצועי" : "Professional Summary";
        var skillsLabel = language == "he" ? "כישורים מרכזיים" : "Core Skills";
        var projectsLabel = language == "he" ? "פרויקטים נבחרים" : "Selected Projects";

        var roleLabel = language == "he" ? "תפקיד יעד: " : "Target Role: ";
        return $@"<!DOCTYPE html>
<html lang=""{language}"" dir=""{direction}"">
<head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  <title>{encodedTitle}</title>
  <style>
    body {{ font-family: {font}; margin: 0; background: #f8fafc; color: #111827; }}
    .resume {{ max-width: 920px; margin: 24px auto; background: #fff; padding: 28px 34px; border-radius: 14px; box-shadow: 0 6px 24px rgba(15, 23, 42, .08); }}
    .header {{ border-bottom: 3px solid {accent}; padding-bottom: 12px; margin-bottom: 16px; }}
    .name {{ font-size: 32px; font-weight: 700; margin: 0; color: {accent}; }}
    .meta {{ font-size: 14px; margin-top: 6px; color: #334155; }}
    .role {{ margin-top: 10px; font-size: 16px; font-weight: 600; }}
    h2 {{ color: {accent}; font-size: 20px; margin: 18px 0 8px; }}
    p {{ line-height: 1.6; margin: 6px 0; }}
    ul {{ margin: 8px 0; padding-inline-start: 20px; }}
    li {{ margin-bottom: 4px; }}
    .item {{ margin-bottom: 12px; }}
    .item h3 {{ margin: 0 0 4px; font-size: 16px; }}
    .github {{ font-size: 13px; color: {accent}; text-decoration: none; }}
  </style>
</head>
<body>
  <main class=""resume"">
    <section class=""header"">
      <h1 class=""name"">{encodedName}</h1>
      <div class=""meta"">{encodedEmail} | {encodedPhone} | {encodedAddress}</div>
      <div class=""role"">{roleLabel}{encodedTargetRole}</div>
    </section>

    <section>
      <h2>{summaryLabel}</h2>
      <p>{summary}</p>
    </section>

    <section>
      <h2>{skillsLabel}</h2>
      <ul>{skillsHtml}</ul>
    </section>

    <section>
      <h2>{projectsLabel}</h2>
      {projectsHtml}
    </section>
  </main>
</body>
</html>";
    }

    private static string BuildSummary(string bio, string targetRole, string language, string tone)
    {
        var cleanedBio = TrimSentence(bio, language);
        var baseSummary = language == "he"
            ? $"מועמד/ת לתפקיד {targetRole} עם גישה {TranslateTone(tone)}, יכולת הובלה וביצוע, וניסיון בבניית פתרונות מקצה לקצה."
            : $"Targeting {targetRole} with a {tone.ToLowerInvariant()} approach, strong ownership mindset, and hands-on execution across full solution delivery.";

        if (string.IsNullOrWhiteSpace(cleanedBio))
        {
            return baseSummary;
        }

        return language == "he"
            ? $"{baseSummary} {cleanedBio}"
            : $"{baseSummary} {cleanedBio}";
    }

    private static string TranslateTone(string tone) =>
        tone switch
        {
            "Creative" => "יצירתית",
            "Casual" => "נעימה",
            _ => "מקצועית"
        };

    private static string TrimSentence(string? input, string language)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return language == "he"
                ? "מידע נוסף יושלם בהמשך בהתאם לנתוני המשתמש."
                : "Additional profile details can be expanded as the user adds more data.";
        }

        return input.Trim();
    }

    private static string BuildGitHubSuffix(string? gitHubLink, string language)
    {
        if (string.IsNullOrWhiteSpace(gitHubLink))
        {
            return string.Empty;
        }

        return language == "he" ? $" | גיטהאב: {gitHubLink}" : $" | GitHub: {gitHubLink}";
    }

    private static string BuildGitHubAnchor(string? gitHubLink, string language)
    {
        if (string.IsNullOrWhiteSpace(gitHubLink))
        {
            return string.Empty;
        }

        var label = language == "he" ? "קישור לפרויקט" : "Project link";
        return $"<a class='github' href='{Encode(gitHubLink)}' target='_blank' rel='noreferrer noopener'>{label}</a>";
    }

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}
