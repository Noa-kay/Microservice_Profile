namespace ProfilMicroservice.DTOs;

public class ChatHistoryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IEnumerable<ChatMessageDto> Messages { get; set; } = new List<ChatMessageDto>();
}

