using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteTakingApp.Data;
using NoteTakingApp.Models;
using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Controllers;

[Authorize]
public class NoteController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IdentityUser _user;

    public NoteController(ApplicationDbContext context, IdentityUser user)
    {
        _context = context;
        _user = user;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notes = _context.Notes.Where(x => x.UserId == userId && x.Status == Status.Published)
            .Include(x => x.NoteTags)
            .ThenInclude(x => x.Tag)
            .OrderByDescending(x => x.UpdatedDate)
            .ToList();

        var uniqueTags = notes
            .SelectMany(x => x.NoteTags)
            .Select(x => x.Tag)
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .ToList();

        var ViewModel = new HomePage()
        {
            Notes = notes,
            Tags = uniqueTags
        };

        return View(ViewModel);
    }

    public IActionResult ArchivedPages()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notes = _context.Notes.Where(x => x.UserId == userId && x.Status == Status.Archived)
            .Include(x => x.NoteTags)
            .ThenInclude(x => x.Tag)
            .OrderByDescending(x => x.UpdatedDate)
            .ToList();

        var uniqueTags = notes
            .SelectMany(x => x.NoteTags)
            .Select(x => x.Tag)
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .ToList();

        var viewModel = new HomePage()
        {
            Notes = notes,
            Tags = uniqueTags
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult CreateNote(Note note, string tags)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        note.UserId = userId;
        note.CreatedDate = DateTime.Now;
        note.UpdatedDate = DateTime.Now;
        _context.Notes.Add(note);
        _context.SaveChanges();

        if (!string.IsNullOrWhiteSpace(tags))
        {
            var multipleTags = tags.Split(",")
                .Select(x => x.Trim())
                .ToList();

            foreach (var tagName in multipleTags)
            {
                var tag = _context.Tags.FirstOrDefault(x => x.Name == tagName && userId == userId);

                if (tag == null)
                {
                    _context.Tags.Add(new Tag { Name = tagName, UserId = userId });
                    _context.SaveChanges();
                }

                var noteTag = new NoteTag
                {
                    NoteId = note.Id,
                    TagId = tag.Id,
                };
                _context.NoteTags.Add(noteTag);
                _context.SaveChanges();
            }
        }

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult UpdateNote(Note note, string tags)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notes = _context.Notes.FirstOrDefault(x => x.Id == note.Id && x.UserId == userId);
        
        if (notes == null)
        {
            return NotFound(new { message = "Note not found." });
        }

        notes.UpdatedDate = DateTime.Now;
        _context.Notes.Update(notes);
        _context.SaveChanges();


        var multipleTags = tags.Split(",")
            .Select(x => x.Trim())
            .ToList();

        var multipleTagRemove = note.NoteTags.Where(x => multipleTags.Contains(x.Tag.Name)).ToList();

        foreach (var tagName in multipleTagRemove)
        {
            note.NoteTags.Remove(tagName);
        }

        foreach (var tagName in multipleTags)
        {
            var tag = _context.Tags.FirstOrDefault(x => x.Name == tagName && userId == userId);

            if (tag == null)
            {
                _context.Tags.Update(new Tag { Name = tagName, UserId = userId });
                _context.SaveChanges();
            }

            var noteTag = new NoteTag
            {
                NoteId = note.Id,
                TagId = tag.Id,
            };
            _context.NoteTags.Add(noteTag);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ArchiveNote(int noteId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notes = _context.Notes.FirstOrDefault(x => x.Id == noteId && x.UserId == userId);
        
        notes.Status = Status.Archived;
        _context.Notes.Update(notes);
        _context.SaveChanges();
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult RecoverNote(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notes = _context.Notes.FirstOrDefault(x => x.Id == id && x.UserId == userId);

        notes.Status = Status.Published;
        _context.Notes.Update(notes);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteNote(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notes = _context.Notes.FirstOrDefault(x => x.Id == id && x.UserId == userId);

        notes.Status = Status.Deleted;
        _context.Notes.Update(notes);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public IActionResult Search(string searchString)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notes = _context.Notes
            .Where(x => x.UserId == userId && ( x.Title.Contains(searchString) || x.Content.Contains(searchString) || x.NoteTags.Any(x => x.Tag.Name.Contains(searchString))))
            .Include(x => x.NoteTags)
            .ThenInclude(x => x.Tag)
            .OrderByDescending(x => x.UpdatedDate)
            .ToList();
        
        var viewModel = new SearchResult()
        {
            SearchString = searchString,
            Notes = notes,
            Tags = _context.Tags.Where(x => x.NoteTags.Any(x => x.Note.UserId == userId)).ToList()
        };
        
        return View(viewModel);
    }
}