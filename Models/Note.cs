using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NoteTakingApp.Models;

public class Note
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot be more than 200 characters")]
    public string Title { get; set; }
    
    [Required(ErrorMessage = "Content is required")]
    [MaxLength(100000, ErrorMessage = "Content cannot be more than 100000 characters")]
    public string Content { get; set;}
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    
    public Status Status { get; set; } = Status.Published;
    
    public ICollection<NoteTag> NoteTags { get; set; }
    
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public IdentityUser User { get; set; }
}

public enum Status
{
    Published = 1,
    Archived = 2,
    Deleted = 3
}