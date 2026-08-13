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
}
