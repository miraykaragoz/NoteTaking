using NoteTakingApp.Models;

namespace NoteTakingApp.ViewModels;

public class HomePage
{
    public List<Note> Notes { get; set; } = new List<Note>();
    public List<Tag> Tags { get; set; } = new List<Tag>();
}