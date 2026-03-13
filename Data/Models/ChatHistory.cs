namespace student_profile.Data.Models;

public class ChatHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}


