using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NoteTakingApp.Models;

namespace NoteTakingApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}