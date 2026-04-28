namespace student_profile.BLL.Interfaces;

public interface IStaff6EventPublisher
{
    Task PublishStudentAcceptedAsync(Guid userId, CancellationToken cancellationToken = default);
}
