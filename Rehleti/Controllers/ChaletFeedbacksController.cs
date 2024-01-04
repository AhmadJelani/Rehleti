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
    public class ChaletFeedbacksController : Controller
    {
        private readonly ModelContext _context;

        public ChaletFeedbacksController(ModelContext context)
        {
            _context = context;
        }
        // GET: ChaletFeedbacks/Create
        public IActionResult Create(int? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (id == null)
            {
                return NotFound();
            }
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            ViewData["ChaletId"] = new SelectList(_context.Chalets, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "Id");
            return View();
        }

        // POST: ChaletFeedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id,[Bind("Id,ChaletId,Text,UserId,Rate")] ChaletFeedback chaletFeedback,float rating)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (ModelState.IsValid)
            {
                if (id==null)
                {
                    return NotFound();
                }
                chaletFeedback.UserId = HttpContext.Session.GetInt32("UserID");
                chaletFeedback.ChaletId = id;
                var validFeedback = await _context.ChaletFeedbacks.Where(x => x.UserId == chaletFeedback.UserId && x.ChaletId == id).AsNoTracking().FirstOrDefaultAsync();
                if (validFeedback != null)
                {
                    ViewData["Error"] = "Your Feedback Is Submitted";
                    return View(validFeedback);
                }
                chaletFeedback.Rate = (decimal)rating;
                _context.Add(chaletFeedback);
                await _context.SaveChangesAsync();

                var chalet = await _context.Chalets.Where(x => x.Id == id).FirstOrDefaultAsync();
                var feedbacks = await _context.ChaletFeedbacks.Where(x => x.ChaletId == id).ToListAsync();

                int NumberOfFeedbacks = feedbacks.Count;
                decimal SummationOfTheRate = (decimal)feedbacks.Sum(x => x.Rate);
                decimal averageRating = NumberOfFeedbacks > 0 ? SummationOfTheRate / NumberOfFeedbacks : 0;
                chalet.Rating= averageRating;

                _context.Update(chalet);
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),"MyTrips");
            }
            ViewData["ChaletId"] = new SelectList(_context.Chalets, "Id", "Id", chaletFeedback.ChaletId);
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "Id", chaletFeedback.UserId);
            return View(chaletFeedback);
        }
    }
}
