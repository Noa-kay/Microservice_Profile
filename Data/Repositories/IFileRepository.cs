using student_profile.Data.Models;

namespace student_profile.Data.Repositories;

public interface IFileRepository
{
    Task<IEnumerable<student_profile.Data.Models.UserFile>> GetAllDocumentsAsync(int userId, CancellationToken ct = default);
    Task SaveFileAsync(student_profile.Data.Models.UserFile file, CancellationToken ct = default);
    Task DeleteDocumentAsync(int documentId, CancellationToken ct = default);
    Task UpdateDocumentDetailsAsync(int documentId, student_profile.Data.Models.UserFile newData, CancellationToken ct = default);
}