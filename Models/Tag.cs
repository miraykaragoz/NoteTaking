using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NoteTakingApp.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200, ErrorMessage = "Name cannot be more than 200 characters")]
    public string Name { get; set; }
    
    public ICollection<NoteTag> NoteTags { get; set; }
    
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public IdentityUser User { get; set; }
}