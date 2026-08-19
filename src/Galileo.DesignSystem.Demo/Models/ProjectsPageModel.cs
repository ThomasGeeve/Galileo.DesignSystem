namespace Galileo.DesignSystem.Demo.Models;

public sealed class ProjectsPageModel
{
    public IReadOnlyList<DemoProject> Projects { get; init; } = [];

    public IReadOnlyList<string> Clients { get; init; } = [];

    public string Search { get; init; } = string.Empty;

    public string? SelectedClient { get; init; }

    public int CurrentPage { get; init; } = 1;

    public int TotalPages { get; init; }

    public int TotalItems { get; init; }

    public int PageSize { get; init; } = 10;
}

public sealed record DemoProject(
    int ProjectId,
    string ProjectNumber,
    string Name,
    string Client,
    string Location,
    string Status,
    string Type,
    DateTime? StartDate,
    DateTime? EndDate)
{
    private static readonly string[] ContactPersons =
    [
        "Sander de Boer",
        "Marieke van Dijk",
        "Jeroen Meijer",
        "Nina Bakker"
    ];

    private static readonly string[] ContactEmails =
    [
        "sander.deboer@example.com",
        "marieke.vandijk@example.com",
        "jeroen.meijer@example.com",
        "nina.bakker@example.com"
    ];

    private static readonly string[] ContactPhoneNumbers =
    [
        "06-12345678",
        "06-23456789",
        "06-34567890",
        "06-45678901"
    ];

    public string Address => $"Gemaalweg {ProjectId}, {Location}";

    public string ContactPerson => ContactPersons[(ProjectId - 1) % ContactPersons.Length];

    public string ContactEmail => ContactEmails[(ProjectId - 1) % ContactEmails.Length];

    public string ContactPhoneNumber => ContactPhoneNumbers[(ProjectId - 1) % ContactPhoneNumbers.Length];

    public string GemaalName => $"Gemaal {Location}";

    public string GemaalNumber => $"GM-{ProjectId:000}";
}

public sealed class ProjectDetailsPageModel
{
    public required DemoProject Project { get; init; }

    public IReadOnlyList<DemoWorkLog> WorkLogs { get; init; } = [];

    public string WorkLogSearch { get; init; } = string.Empty;

    public int CurrentPage { get; init; } = 1;

    public int TotalPages { get; init; }

    public int TotalItems { get; init; }

    public int PageSize { get; init; } = 10;
}

public sealed record DemoWorkLog(
    int WorkLogId,
    string PerformedBy,
    DateOnly Date,
    string TravelTimeTo,
    string WorkDuration,
    string TravelTimeFrom);
