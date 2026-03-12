using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public interface IChatRepository
{
    Task<ChatHistory> InitializeChatSessionAsync(int userId, CancellationToken ct = default);
    Task<ChatHistory?> GetChatHistoryAsync(int userId, CancellationToken ct = default);
    Task<Message> SaveMessageAsync(Message message, CancellationToken ct = default);
}