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

    public IActionResult Molecules()
    {
        return View();
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
}
