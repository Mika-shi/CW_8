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

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        int pageSize = 10;

        int totalTopics = await _context.Topics.CountAsync();

        int totalPages = (int)Math.Ceiling(totalTopics / (double)pageSize);

        if (page < 1)
        {
            page = 1;
        }

        if (page > totalPages && totalPages > 0)
        {
            page = totalPages;
        }

        var topics = await _context.Topics
            .Include(topic => topic.User)
            .Include(topic => topic.Replies)
            .OrderByDescending(topic => topic.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(topics);
    }
}