using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteTakingApp.Models;

public class NoteTag
{
    [Key]
    public int Id { get; set; }
    
    public int NoteId { get; set; }
    [ForeignKey("NoteId")]
    public Note Note { get; set; }
    
    public int TagId { get; set; }
    [ForeignKey("TagId")]
    public Tag Tag { get; set; }
}