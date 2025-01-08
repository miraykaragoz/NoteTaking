using NoteTakingApp.Models;

namespace NoteTakingApp.ViewModels;

public class SearchResult
{
    public string SearchString { get; set; }
    public List<Note> Notes { get; set; }
    public List<Tag> Tags { get; set; }
}