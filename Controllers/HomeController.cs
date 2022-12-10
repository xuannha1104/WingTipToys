using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WingtipToys.Models;

namespace WingtipToys.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProductContext _context;

    public HomeController(ILogger<HomeController> logger,ProductContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var categories = _context.Categories.AsQueryable();

        ViewBag.categories = categories;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
