using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        Topic? topic = await _context.Topics
            .Include(topic => topic.User)
            .Include(topic => topic.Replies)
            .ThenInclude(reply => reply.User)
            .FirstOrDefaultAsync(topic => topic.Id == id);

        if (topic == null)
        {
            return NotFound();
        }

        return View(topic);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddReply(int topicId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Json(new
            {
                success = false,
                error = "Введите текст ответа"
            });
        }

        Topic? topic = await _context.Topics.FindAsync(topicId);

        if (topic == null)
        {
            return Json(new
            {
                success = false,
                error = "Тема не найдена"
            });
        }

        string? userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return Json(new
            {
                success = false,
                error = "Пользователь не авторизован"
            });
        }

        User? user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Json(new
            {
                success = false,
                error = "Пользователь не найден"
            });
        }

        Reply reply = new Reply
        {
            Text = text,
            CreatedOn = DateTime.Now,
            TopicId = topicId,
            UserId = userId
        };

        _context.Replies.Add(reply);

        user.MessagesCount++;

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            id = reply.Id,
            text = reply.Text,
            createdOn = reply.CreatedOn.ToString("dd.MM.yyyy HH:mm"),
            userId = user.Id,
            userName = user.UserName,
            avatarPath = user.AvatarPath,
            messagesCount = user.MessagesCount
        });
    }
}