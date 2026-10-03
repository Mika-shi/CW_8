namespace MyForum.Models;

public class Topic
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Content { get; set; } = "";

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public string UserId { get; set; } = "";

    public User? User { get; set; }

    public List<Reply> Replies { get; set; } = new();
}