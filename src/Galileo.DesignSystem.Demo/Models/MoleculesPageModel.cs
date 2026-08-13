namespace Galileo.DesignSystem.Demo.Models;

public sealed class MoleculesPageModel
{
    public IReadOnlyList<DemoUser> Users { get; init; } = [];

    public int CurrentPage { get; init; }

    public int TotalPages { get; init; }

    public int TotalItems { get; init; }

    public int PageSize { get; init; }
}

public sealed record DemoUser(int Id, string Name, string Email, string Role);
