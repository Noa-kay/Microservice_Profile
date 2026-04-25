namespace student_profile.Data.Models;

public class Message
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public Guid SenderId { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public Guid ChatHistoryId { get; set; }

    public User Sender { get; set; } = null!;
    public ChatHistory ChatHistory { get; set; } = null!;
}

