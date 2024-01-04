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
    public class ChaletFeedbacksController : Controller
    {
        private readonly ModelContext _context;

        public ChaletFeedbacksController(ModelContext context)
        {
            _context = context;
        }

        // GET: Dashboard/ChaletFeedbacks
        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            var modelContext = _context.ChaletFeedbacks.Include(c => c.Chalet).Include(c => c.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Dashboard/ChaletFeedbacks/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.ChaletFeedbacks == null)
            {
                return NotFound();
            }

            var chaletFeedback = await _context.ChaletFeedbacks
                .Include(c => c.Chalet)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chaletFeedback == null)
            {
                return NotFound();
            }

            return View(chaletFeedback);
        }

        // GET: Dashboard/ChaletFeedbacks/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.ChaletFeedbacks == null)
            {
                return NotFound();
            }

            var chaletFeedback = await _context.ChaletFeedbacks
                .Include(c => c.Chalet)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chaletFeedback == null)
            {
                return NotFound();
            }

            return View(chaletFeedback);
        }

        // POST: Dashboard/ChaletFeedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.ChaletFeedbacks == null)
            {
                return Problem("Entity set 'ModelContext.ChaletFeedbacks'  is null.");
            }
            var chaletFeedback = await _context.ChaletFeedbacks.FindAsync(id);
            if (chaletFeedback != null)
            {
                _context.ChaletFeedbacks.Remove(chaletFeedback);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChaletFeedbackExists(decimal id)
        {
          return (_context.ChaletFeedbacks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
