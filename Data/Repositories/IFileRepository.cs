using student_profile.Data.Models;

namespace student_profile.Data.Repositories
{
    public interface IFileRepository
    {
        Task<IEnumerable<UserFile>> GetAllDocumentsAsync(Guid userId, CancellationToken ct = default);
        Task SaveFileAsync(UserFile file, CancellationToken ct = default);
        Task DeleteDocumentAsync(Guid documentId, CancellationToken ct = default);
        Task UpdateDocumentDetailsAsync(Guid documentId, UserFile newData, CancellationToken ct = default);
    }
}