namespace student_profile.DTOs;

public class ChatHistoryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

public class MessageDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public Guid SenderId { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public Guid ChatHistoryId { get; set; }
}

