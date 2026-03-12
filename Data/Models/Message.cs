namespace student_profile.Data.Models;

public class Message
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int SenderId { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public int ChatHistoryId { get; set; }

    public User Sender { get; set; } = null!;
    public ChatHistory ChatHistory { get; set; } = null!;
}

