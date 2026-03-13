namespace ProfilMicroservice.DTOs;

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Sender { get; set; } = default!;
    public string Message { get; set; } = default!;
}

