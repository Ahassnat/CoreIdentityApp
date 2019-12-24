using System.IO;
using Microsoft.AspNetCore.Mvc;

// this controller is for routhing the SPA angular in Path we decleare it in Startup.cs
namespace DatingApp.API.Controllers
{
    public class Fallback: Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot","index.html"), "text/HTML");
        }
    }
}