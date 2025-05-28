using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.Models;

namespace PitStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Get featured teams
        ViewBag.FeaturedTeams = await _context.Teams
            .OrderBy(t => t.Name)
            .Take(6)
            .ToListAsync();

        // Get featured products
        ViewBag.FeaturedProducts = await _context.Products
            .Include(p => p.Team)
            .Where(p => p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();

        // Get categories
        ViewBag.Categories = await _context.Categories
            .OrderBy(c => c.Name)
            .Take(6)
            .ToListAsync();

        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
