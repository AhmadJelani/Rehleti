using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;

namespace Rehleti.Controllers
{
    public class TripFeedbacksController : Controller
    {
        private readonly ModelContext _context;

        public TripFeedbacksController(ModelContext context)
        {
            _context = context;
        }

        // GET: TripFeedbacks/Create
        public IActionResult Create(int id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            ViewData["TripId"] = new SelectList(_context.AdventureTrips, "Id", "Id");
            return View();
        }

        // POST: TripFeedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id,[Bind("Id,TripId,Text,UserId")] TripFeedback tripFeedback)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (ModelState.IsValid)
            {
                tripFeedback.TripId = id;
                tripFeedback.UserId = HttpContext.Session.GetInt32("UserID");
                var validFeedback = await _context.TripFeedbacks.Where(x => x.UserId == tripFeedback.UserId && x.TripId == tripFeedback.TripId).AsNoTracking().FirstOrDefaultAsync();
                if (validFeedback!=null)
                {
                    ViewData["Error"] = "Your Feedback Is Submitted";
                    return View(validFeedback);
                }
                _context.Add(tripFeedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),"Home");
            }
            ViewData["TripId"] = new SelectList(_context.AdventureTrips, "Id", "Id", tripFeedback.TripId);
            return View(tripFeedback);
        }
        private bool TripFeedbackExists(decimal id)
        {
          return (_context.TripFeedbacks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
