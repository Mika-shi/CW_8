using System.ComponentModel.DataAnnotations;

namespace MyForum.ViewModels;

public class TopicCreateViewModel
{
    [Required(ErrorMessage = "Введите название темы")]
    [Display(Name = "Название темы")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Введите содержание темы")]
    [Display(Name = "Содержание")]
    public string Content { get; set; } = "";
}