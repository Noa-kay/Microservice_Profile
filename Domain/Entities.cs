namespace ProfilMicroservice.Domain;

public class User
{
    public Guid Id { get; set; }
}

public class PersonalDetails
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Bio { get; set; } = default!;
}

public class Project
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ProjectName { get; set; } = default!;
    public string GitHubLink { get; set; } = default!;
}

public class UserFile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool Uploaded { get; set; }
    public DateTime? UploadDate { get; set; }
    public string FileType { get; set; } = default!;
    public string Url { get; set; } = default!;
}

public class Image
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ImageName { get; set; } = default!;
    public string Url { get; set; } = default!;
}

public class Skill
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}

public class Category
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = default!;
}

public class SkillToUser
{
    public Guid UserId { get; set; }
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = default!;
    public int? YearsOfExperience { get; set; }
}

public class ChatHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IEnumerable<Message> Messages { get; set; } = new List<Message>();
}

public class Message
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Sender { get; set; } = default!;
    public string Massage { get; set; } = default!;
}

