using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _context;
    public ChatRepository(AppDbContext context) => _context = context;

    public async Task<ChatHistory> InitializeChatSessionAsync(int userId, CancellationToken cancellationToken = default)
    {
        var existingChat = await _context.ChatHistories
            .Include(ch => ch.Messages)
            .FirstOrDefaultAsync(ch => ch.UserId == userId, cancellationToken);

        if (existingChat is not null) return existingChat;

        var chatHistory = new ChatHistory { UserId = userId };
        _context.ChatHistories.Add(chatHistory);
        await _context.SaveChangesAsync(cancellationToken);

        return await _context.ChatHistories
            .Include(ch => ch.Messages)
            .FirstAsync(ch => ch.Id == chatHistory.Id, cancellationToken);
    }

    public async Task<ChatHistory?> GetChatHistoryAsync(int userId, CancellationToken cancellationToken = default) =>
        await _context.ChatHistories
            .Include(ch => ch.Messages.OrderBy(m => m.Date))
            .FirstOrDefaultAsync(ch => ch.UserId == userId, cancellationToken);

    public async Task<Message> SaveMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        if (message.Date == default) message.Date = DateTime.UtcNow;
        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);
        return message;
    }
}