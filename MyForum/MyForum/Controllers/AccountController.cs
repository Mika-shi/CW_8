using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyForum.Models;
using MyForum.ViewModels;

namespace MyForum.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IWebHostEnvironment _environment;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        User? userWithSameName = await _userManager.FindByNameAsync(model.UserName);

        if (userWithSameName != null)
        {
            ModelState.AddModelError("UserName", "Пользователь с таким именем уже существует");
        }

        User? userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);

        if (userWithSameEmail != null)
        {
            ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
        }

        DateTime today = DateTime.Today;
        int age = today.Year - model.BirthDate.Year;

        if (model.BirthDate.Date > today.AddYears(-age))
        {
            age--;
        }

        if (age < 18)
        {
            ModelState.AddModelError("BirthDate", "Регистрация доступна только пользователям старше 18 лет");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? avatarPath = null;

        if (model.Avatar != null)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string extension = Path.GetExtension(model.Avatar.FileName);
            string fileName = Guid.NewGuid() + extension;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using FileStream stream = new FileStream(filePath, FileMode.Create);
            await model.Avatar.CopyToAsync(stream);

            avatarPath = "/uploads/" + fileName;
        }

        User user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            BirthDate = model.BirthDate,
            AvatarPath = avatarPath,
            MessagesCount = 0
        };

        IdentityResult result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }

        foreach (IdentityError error in result.Errors)
        {
            string message = error.Code switch
            {
                "PasswordTooShort" => "Пароль должен содержать минимум 6 символов",
                "PasswordRequiresUpper" => "Пароль должен содержать минимум одну заглавную букву",
                "PasswordRequiresLower" => "Пароль должен содержать минимум одну строчную букву",
                "PasswordRequiresDigit" => "Пароль должен содержать минимум одну цифру",
                _ => "Ошибка регистрации"
            };

            ModelState.AddModelError("", message);
        }

        return View(model);
    }
}