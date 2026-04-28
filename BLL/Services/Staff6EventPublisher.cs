using System.Net.Http.Json;
using student_profile.BLL.Interfaces;
using student_profile.DTOs;

namespace student_profile.BLL.Services;

public class Staff6EventPublisher : IStaff6EventPublisher
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Staff6EventPublisher> _logger;

    public Staff6EventPublisher(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<Staff6EventPublisher> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task PublishStudentAcceptedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var endpoint = _configuration["Staff6Events:Endpoint"];
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            _logger.LogWarning("Staff6 event was skipped because Staff6Events:Endpoint is missing.");
            return;
        }

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
        {
            _logger.LogWarning("Staff6 event was skipped because endpoint is invalid: {Endpoint}", endpoint);
            return;
        }

        var apiKey = _configuration["Staff6Events:ApiKey"];
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Remove("X-Api-Key");
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        }

        var payload = new ApplicationStatusChangedEvent(
            StudentId: userId.ToString(),
            NewStatus: "Accepted",
            ChangedAtUtc: DateTime.UtcNow);

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(endpointUri, payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Staff6 event failed with status {StatusCode}. Body: {Body}",
                    (int)response.StatusCode,
                    body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish student.accepted event to Staff6.");
        }
    }
}
