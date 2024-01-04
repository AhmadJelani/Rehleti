using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;

namespace Rehleti.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class BookTripsController : Controller
    {
        private readonly ModelContext _context;

        public BookTripsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Dashboard/BookTrips
        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            var modelContext = _context.BookTrips.Include(b => b.AdventureTrip).Include(b => b.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Dashboard/BookTrips/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.BookTrips == null)
            {
                return NotFound();
            }

            var bookTrip = await _context.BookTrips
                .Include(b => b.AdventureTrip)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookTrip == null)
            {
                return NotFound();
            }

            return View(bookTrip);
        }

        private bool BookTripExists(decimal id)
        {
          return (_context.BookTrips?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
