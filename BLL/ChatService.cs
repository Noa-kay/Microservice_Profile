using Microsoft.EntityFrameworkCore;
using student_profile.Data.Context;
using student_profile.Data.Models;
using student_profile.DTOs;

namespace student_profile.BLL;

public interface IChatService
{
    Task<ChatHistoryDto> InitializeChatSessionAsync(Guid userId, CancellationToken ct = default);
    Task<ChatHistoryDto?> GetChatHistoryAsync(Guid userId, CancellationToken ct = default);
    Task<MessageDto> SaveMessageAsync(MessageDto message, CancellationToken ct = default);
}

public class ChatService : IChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ChatHistoryDto> InitializeChatSessionAsync(Guid userId, CancellationToken ct = default)
    {
        var existing = await _context.ChatHistories
            .FirstOrDefaultAsync(ch => ch.UserId == userId, ct);

        if (existing is not null)
        {
            return new ChatHistoryDto
            {
                Id = existing.Id,
                UserId = existing.UserId
            };
        }

        var entity = new ChatHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };

        _context.ChatHistories.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new ChatHistoryDto
        {
            Id = entity.Id,
            UserId = entity.UserId
        };
    }

    public async Task<ChatHistoryDto?> GetChatHistoryAsync(Guid userId, CancellationToken ct = default)
    {
        var entity = await _context.ChatHistories.AsNoTracking()
            .FirstOrDefaultAsync(ch => ch.UserId == userId, ct);

        if (entity is null)
        {
            return null;
        }

        return new ChatHistoryDto
        {
            Id = entity.Id,
            UserId = entity.UserId
        };
    }

    public async Task<MessageDto> SaveMessageAsync(MessageDto message, CancellationToken ct = default)
    {
        var entity = new Message
        {
            Id = message.Id == Guid.Empty ? Guid.NewGuid() : message.Id,
            Date = message.Date,
            SenderId = message.SenderId,
            MessageContent = message.MessageContent,
            ChatHistoryId = message.ChatHistoryId
        };

        _context.Messages.Add(entity);
        await _context.SaveChangesAsync(ct);

        message.Id = entity.Id;
        return message;
    }
}

