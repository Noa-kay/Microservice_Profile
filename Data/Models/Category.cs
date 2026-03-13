namespace student_profile.Data.Models;

public class Category
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
