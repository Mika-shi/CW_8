using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyForum.Data;
using MyForum.Models;
using MyForum.ViewModels;

namespace MyForum.Controllers;

public class TopicController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public TopicController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(TopicCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Unauthorized();
        }

        Topic topic = new Topic
        {
            Title = model.Title,
            Content = model.Content,
            CreatedOn = DateTime.Now,
            UserId = userId
        };

        _context.Topics.Add(topic);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}