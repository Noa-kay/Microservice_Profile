namespace student_profile.DTOs;

public record ApplicationStatusChangedEvent(
    string StudentId,
    string NewStatus,
    DateTime ChangedAtUtc
);
