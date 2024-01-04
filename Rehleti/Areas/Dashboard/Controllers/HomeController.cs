using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;
using System.Net.NetworkInformation;

namespace Rehleti.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : Controller
    {
        private readonly ModelContext _context;
        public HomeController(ModelContext context)
        {
            _context = context;
        }
        public async Task <IActionResult> Index()
        {
			ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
			ViewData["Name"] = HttpContext.Session.GetString("FirstName")+" "+ HttpContext.Session.GetString("LastName");
			ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID")==null)
            {
                return RedirectToAction("SignIn", "Account", new { area=""});
            }
            ViewData["TotalUsers"] = await _context.UserGuests.Where(x => x.RoleId == 22).CountAsync();
            ViewData["TotalChalets"] = await _context.Chalets.CountAsync();
            ViewData["TotalTrips"] = await _context.AdventureTrips.CountAsync();
            return View();
        }
        
    }
}
