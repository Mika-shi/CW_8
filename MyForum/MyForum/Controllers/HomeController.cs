using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyForum.Data;

namespace MyForum.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var topics = await _context.Topics
            .Include(topic => topic.User)
            .Include(topic => topic.Replies)
            .OrderByDescending(topic => topic.CreatedOn)
            .ToListAsync();

        return View(topics);
    }
}