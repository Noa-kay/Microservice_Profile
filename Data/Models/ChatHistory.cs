namespace student_profile.Data.Models;

public class ChatHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}


