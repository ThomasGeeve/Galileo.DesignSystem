using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Galileo.DesignSystem.Demo.Models;

namespace Galileo.DesignSystem.Demo.Controllers;

public class HomeController(DemoProjectStore projectStore) : Controller
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
        var allProjects = projectStore.GetAll();
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
        var project = projectStore.Get(projectId);
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

    [HttpPost("/Home/Projects/{projectId:int}/name")]
    public IActionResult UpdateProjectName(int projectId, [FromForm] UpdateProjectNameRequest request)
    {
        var project = projectStore.Get(projectId);
        if (project is null)
            return NotFound(new { message = "Project niet gevonden." });

        var name = request.Name?.Trim() ?? string.Empty;
        var errors = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(name))
        {
            errors[nameof(UpdateProjectNameRequest.Name)] = ["Vul een projectnaam in."];
        }
        else if (name.Length > 120)
        {
            errors[nameof(UpdateProjectNameRequest.Name)] = ["Een projectnaam mag maximaal 120 tekens bevatten."];
        }

        if (errors.Count > 0)
            return BadRequest(new { errors });

        var updatedProject = projectStore.UpdateName(projectId, name);
        if (updatedProject is null)
            return NotFound(new { message = "Project niet gevonden." });

        return Ok(new { value = updatedProject.Name });
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
