namespace MyForum.Models;

public class Reply
{
    public int Id { get; set; }

    public string Text { get; set; } = "";

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public int TopicId { get; set; }

    public Topic? Topic { get; set; }

    public string UserId { get; set; } = "";

    public User? User { get; set; }
}