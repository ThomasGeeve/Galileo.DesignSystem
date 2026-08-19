using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Galileo.DesignSystem.Demo.Models;

namespace Galileo.DesignSystem.Demo.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Atoms()
    {
        return View();
    }

    public IActionResult Molecules(int page = 1)
    {
        const int pageSize = 5;
        var allUsers = CreateDemoUsers();
        var totalPages = (int)Math.Ceiling(allUsers.Count / (double)pageSize);
        var currentPage = Math.Clamp(page, 1, totalPages);
        var pageUsers = allUsers
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return View(new MoleculesPageModel
        {
            Users = pageUsers,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            TotalItems = allUsers.Count,
            PageSize = pageSize
        });
    }

    public IActionResult Organisms()
    {
        return View();
    }

    public IActionResult Projects(string? search, string? client, int page = 1)
    {
        const int pageSize = 10;
        var allProjects = CreateDemoProjects();
        var normalizedSearch = search?.Trim() ?? string.Empty;
        var normalizedClient = client?.Trim() ?? string.Empty;
        var clients = allProjects
            .Select(project => project.Client)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(clientName => clientName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var filteredProjects = allProjects
            .Where(project => string.IsNullOrWhiteSpace(normalizedSearch) ||
                              project.ProjectNumber.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                              project.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            .Where(project => string.IsNullOrWhiteSpace(normalizedClient) ||
                              string.Equals(project.Client, normalizedClient, StringComparison.OrdinalIgnoreCase))
            .OrderBy(project => project.ProjectNumber, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var totalPages = Math.Max(1, (int)Math.Ceiling(filteredProjects.Length / (double)pageSize));
        var currentPage = Math.Clamp(page, 1, totalPages);
        var pageProjects = filteredProjects
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return View(new ProjectsPageModel
        {
            Projects = pageProjects,
            Clients = clients,
            Search = normalizedSearch,
            SelectedClient = string.IsNullOrWhiteSpace(normalizedClient) ? null : normalizedClient,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            TotalItems = filteredProjects.Length,
            PageSize = pageSize
        });
    }

    [HttpGet("/Home/Projects/{projectId:int}")]
    public IActionResult ProjectDetails(int projectId, string? search, int page = 1)
    {
        const int pageSize = 10;
        var project = CreateDemoProjects().SingleOrDefault(item => item.ProjectId == projectId);
        if (project is null)
            return NotFound();

        var allWorkLogs = CreateDemoWorkLogs(project);
        var normalizedSearch = search?.Trim() ?? string.Empty;
        var filteredWorkLogs = allWorkLogs
            .Where(workLog => string.IsNullOrWhiteSpace(normalizedSearch) ||
                              workLog.PerformedBy.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var totalPages = Math.Max(1, (int)Math.Ceiling(filteredWorkLogs.Length / (double)pageSize));
        var currentPage = Math.Clamp(page, 1, totalPages);
        var pageWorkLogs = filteredWorkLogs
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return View(new ProjectDetailsPageModel
        {
            Project = project,
            WorkLogs = pageWorkLogs,
            WorkLogSearch = normalizedSearch,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            TotalItems = filteredWorkLogs.Length,
            PageSize = pageSize
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static IReadOnlyList<DemoUser> CreateDemoUsers()
    {
        return Enumerable.Range(1, 40)
            .Select(id => new DemoUser(
                id,
                $"Gebruiker {id:00}",
                $"user{id:00}@example.com",
                (id % 3) switch
                {
                    0 => "Designer",
                    1 => "Developer",
                    _ => "Product owner"
                }))
            .ToArray();
    }

    private static IReadOnlyList<DemoProject> CreateDemoProjects()
    {
        return
        [
            new(1, "GT-2026-001", "Renovatie gemaal Kinderdijk", "Waterschap Rivierenland", "Kinderdijk", "Actief", "Groot", new DateTime(2026, 1, 12), null),
            new(2, "GT-2026-002", "Onderhoud gemaal De Esch", "Gemeente Rotterdam", "Rotterdam", "In voorbereiding", "Klein", new DateTime(2026, 2, 2), null),
            new(3, "GT-2026-003", "Vervanging besturing gemaal Westland", "Provincie Zuid-Holland", "Naaldwijk", "Actief", "Groot", new DateTime(2026, 1, 26), null),
            new(4, "GT-2026-004", "Inspectie gemaal De Wetering", "Waterschap Rivierenland", "Tiel", "Afgerond", "Klein", new DateTime(2025, 10, 6), new DateTime(2025, 12, 18)),
            new(5, "GT-2026-005", "Calamiteit gemaal Vlietpolder", "Provincie Zuid-Holland", "Leiden", "Actief", "Calamiteit", new DateTime(2026, 2, 16), null),
            new(6, "GT-2026-006", "Renovatie gemaal Zuidland", "Gemeente Rotterdam", "Zuidland", "In voorbereiding", "Groot", new DateTime(2026, 3, 9), null),
            new(7, "GT-2026-007", "Nieuwe pompinstallatie Oude Maas", "Waterschap Rivierenland", "Dordrecht", "Actief", "Groot", new DateTime(2026, 2, 23), null),
            new(8, "GT-2026-008", "Klein onderhoud gemaal Schiedam", "Gemeente Rotterdam", "Schiedam", "Afgerond", "Klein", new DateTime(2025, 11, 3), new DateTime(2026, 1, 30)),
            new(9, "GT-2026-009", "Storingsherstel gemaal Katwijk", "Provincie Zuid-Holland", "Katwijk", "Actief", "Calamiteit", new DateTime(2026, 3, 2), null),
            new(10, "GT-2026-010", "Aanpassing afvoer gemaal De Meije", "Waterschap Rivierenland", "Woerden", "In voorbereiding", "Klein", new DateTime(2026, 4, 13), null),
            new(11, "GT-2026-011", "Renovatie gemaal Brielle", "Gemeente Rotterdam", "Brielle", "Actief", "Groot", new DateTime(2026, 3, 23), null),
            new(12, "GT-2026-012", "Inspectie gemaal Reeuwijk", "Provincie Zuid-Holland", "Reeuwijk", "Afgerond", "Klein", new DateTime(2025, 12, 1), new DateTime(2026, 2, 6)),
            new(13, "GT-2026-013", "Vervanging elektrotechniek gemaal Lage Zwaluwe", "Waterschap Rivierenland", "Lage Zwaluwe", "Actief", "Groot", new DateTime(2026, 4, 6), null),
            new(14, "GT-2026-014", "Calamiteit gemaal Spaanse Polder", "Gemeente Rotterdam", "Rotterdam", "Actief", "Calamiteit", new DateTime(2026, 4, 20), null),
            new(15, "GT-2026-015", "Onderhoud gemaal Maasdijk", "Provincie Zuid-Holland", "Maasdijk", "In voorbereiding", "Klein", new DateTime(2026, 5, 4), null),
            new(16, "GT-2026-016", "Renovatie gemaal Boven-Hardinxveld", "Waterschap Rivierenland", "Hardinxveld-Giessendam", "In voorbereiding", "Groot", new DateTime(2026, 5, 11), null),
            new(17, "GT-2026-017", "Nieuwe aandrijving gemaal Hoek van Holland", "Gemeente Rotterdam", "Hoek van Holland", "Actief", "Groot", new DateTime(2026, 4, 27), null),
            new(18, "GT-2026-018", "Inspectie gemaal Rijnsburg", "Provincie Zuid-Holland", "Rijnsburg", "Afgerond", "Klein", new DateTime(2026, 1, 19), new DateTime(2026, 3, 13)),
            new(19, "GT-2026-019", "Aanleg tijdelijke pomp gemaal Culemborg", "Waterschap Rivierenland", "Culemborg", "Actief", "Calamiteit", new DateTime(2026, 5, 18), null),
            new(20, "GT-2026-020", "Optimalisatie gemaal Pernis", "Gemeente Rotterdam", "Pernis", "In voorbereiding", "Groot", new DateTime(2026, 6, 1), null),
            new(21, "GT-2026-021", "Onderhoud gemaal Zoetermeer", "Provincie Zuid-Holland", "Zoetermeer", "Actief", "Klein", new DateTime(2026, 5, 25), null),
            new(22, "GT-2026-022", "Vervanging krooshekreiniger gemaal Beesd", "Waterschap Rivierenland", "Beesd", "In voorbereiding", "Groot", new DateTime(2026, 6, 8), null),
            new(23, "GT-2026-023", "Calamiteit gemaal Hoekse Waard", "Provincie Zuid-Holland", "Oud-Beijerland", "Actief", "Calamiteit", new DateTime(2026, 6, 15), null),
            new(24, "GT-2026-024", "Renovatie gemaal Rozenburg", "Gemeente Rotterdam", "Rozenburg", "In voorbereiding", "Groot", new DateTime(2026, 6, 22), null),
            new(25, "GT-2026-025", "Energieverbetering gemaal Asperen", "Waterschap Rivierenland", "Asperen", "Actief", "Klein", new DateTime(2026, 7, 6), null),
            new(26, "GT-2026-026", "Onderhoud gemaal Ameide", "Waterschap Rivierenland", "Ameide", "In voorbereiding", "Klein", new DateTime(2026, 7, 13), null),
            new(27, "GT-2026-027", "Renovatie gemaal Leerdam", "Waterschap Rivierenland", "Leerdam", "Actief", "Groot", new DateTime(2026, 7, 20), null)
        ];
    }

    private static IReadOnlyList<DemoWorkLog> CreateDemoWorkLogs(DemoProject project)
    {
        var performers = new[]
        {
            "Sander de Boer",
            "Marieke van Dijk",
            "Jeroen Meijer",
            "Nina Bakker",
            "Olivier Peters"
        };
        var travelTimesTo = new[] { "00:45", "01:10", "00:30", "00:55" };
        var workDurations = new[] { "04:30", "06:00", "03:45", "07:15" };
        var travelTimesFrom = new[] { "00:35", "00:50", "00:25", "01:05" };
        var startDate = DateOnly.FromDateTime(project.StartDate ?? new DateTime(2026, 1, 1));

        return Enumerable.Range(1, 15)
            .Select(index => new DemoWorkLog(
                project.ProjectId * 100 + index,
                performers[(project.ProjectId + index) % performers.Length],
                startDate.AddDays(index * 2),
                travelTimesTo[index % travelTimesTo.Length],
                workDurations[index % workDurations.Length],
                travelTimesFrom[index % travelTimesFrom.Length]))
            .ToArray();
    }
}
