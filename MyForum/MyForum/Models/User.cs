using Microsoft.AspNetCore.Identity;

namespace MyForum.Models;

public class User : IdentityUser
{
    public DateTime BirthDate { get; set; }
    public string? AvatarPath { get; set; }
    public int MessagesCount { get; set; }
}