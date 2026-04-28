namespace MarketingNotificationService.Data.Entities;

/// <summary>
/// Same namespace and property names as the marketing notification service contract.
/// Type name uses underscore because C# does not allow hyphens (e.g. graduates-count).
/// </summary>
public record graduates_count(
    int TotalGraduates,
    DateTime UpdateDate);

 public record ServiceSettings 
{ 
    public string ServiceName { get; init; } 
}

public record RabbitMQSettings 
{ 
    public string Host { get; init; } 
}