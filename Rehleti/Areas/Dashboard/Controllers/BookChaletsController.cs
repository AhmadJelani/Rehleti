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
    public class BookChaletsController : Controller
    {
        private readonly ModelContext _context;

        public BookChaletsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Dashboard/BookChalets
        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            var modelContext = _context.BookChalets.Include(b => b.Chalet).Include(b => b.Date).Include(b => b.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Dashboard/BookChalets/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.BookChalets == null)
            {
                return NotFound();
            }

            var bookChalet = await _context.BookChalets
                .Include(b => b.Chalet)
                .Include(b => b.Date)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookChalet == null)
            {
                return NotFound();
            }

            return View(bookChalet);
        }
        
        private bool BookChaletExists(decimal id)
        {
          return (_context.BookChalets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
