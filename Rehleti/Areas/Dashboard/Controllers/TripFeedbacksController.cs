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
    public class TripFeedbacksController : Controller
    {
        private readonly ModelContext _context;

        public TripFeedbacksController(ModelContext context)
        {
            _context = context;
        }

        // GET: Dashboard/TripFeedbacks
        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            var modelContext = _context.TripFeedbacks.Include(t => t.Trip).Include(t => t.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Dashboard/TripFeedbacks/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.TripFeedbacks == null)
            {
                return NotFound();
            }

            var tripFeedback = await _context.TripFeedbacks
                .Include(t => t.Trip)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tripFeedback == null)
            {
                return NotFound();
            }

            return View(tripFeedback);
        }

        // GET: Dashboard/TripFeedbacks/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.TripFeedbacks == null)
            {
                return NotFound();
            }

            var tripFeedback = await _context.TripFeedbacks
                .Include(t => t.Trip)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tripFeedback == null)
            {
                return NotFound();
            }

            return View(tripFeedback);
        }

        // POST: Dashboard/TripFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.TripFeedbacks == null)
            {
                return Problem("Entity set 'ModelContext.TripFeedbacks'  is null.");
            }
            var tripFeedback = await _context.TripFeedbacks.FindAsync(id);
            if (tripFeedback != null)
            {
                _context.TripFeedbacks.Remove(tripFeedback);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TripFeedbackExists(decimal id)
        {
          return (_context.TripFeedbacks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
