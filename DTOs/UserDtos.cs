namespace student_profile.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
}

/// <summary>
/// Result of creating a user, including total user count after persistence.
/// </summary>
public sealed record UserCreatedResult(UserDto User, int TotalUsers);

