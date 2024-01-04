using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;

namespace Rehleti.Controllers
{
    public class ListOfDatesForChaletsController : Controller
    {
        private readonly ModelContext _context;

        public ListOfDatesForChaletsController(ModelContext context)
        {
            _context = context;
        }

        // GET: ListOfDatesForChalets
        public async Task<IActionResult> Index(int id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            HttpContext.Session.SetInt32("CID", id);
            ViewData["cid"] = (int)HttpContext.Session.GetInt32("CID");
            var modelContext = _context.ListOfDatesForChalets.Include(l => l.Chalet).Where(x => x.ChaletId == id).ToListAsync();
            return View(await modelContext);
        }

        // GET: ListOfDatesForChalets/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.ListOfDatesForChalets == null)
            {
                return NotFound();
            }

            var listOfDatesForChalet = await _context.ListOfDatesForChalets
                .Include(l => l.Chalet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listOfDatesForChalet == null)
            {
                return NotFound();
            }

            return View(listOfDatesForChalet);
        }

        // GET: ListOfDatesForChalets/Create
        public IActionResult Create(int id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            ViewData["ChaletId"] = new SelectList(_context.Chalets, "Id", "Id");
            return View();
        }

        // POST: ListOfDatesForChalets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChaletId,DateFrom,DateTo,Status")] ListOfDatesForChalet listOfDatesForChalet,int id)
        {
            
            listOfDatesForChalet.ChaletId = id;
            listOfDatesForChalet.Status= "Unbooked";
            var date = new ListOfDatesForChalet() {
                ChaletId= id,DateFrom=listOfDatesForChalet.DateFrom,DateTo=listOfDatesForChalet.DateTo,
                Status = "Unbooked"
        };
            _context.Add(date);
            await _context.SaveChangesAsync();
            return RedirectToAction("YourChalet", "AccountInfo");

        }
        // GET: ListOfDatesForChalets/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.ListOfDatesForChalets == null)
            {
                return NotFound();
            }

            var listOfDatesForChalet = await _context.ListOfDatesForChalets
                .Include(l => l.Chalet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listOfDatesForChalet == null)
            {
                return NotFound();
            }

            return View(listOfDatesForChalet);
        }

        // POST: ListOfDatesForChalets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.ListOfDatesForChalets == null)
            {
                return Problem("Entity set 'ModelContext.ListOfDatesForChalets'  is null.");
            }
            var listOfDatesForChalet = await _context.ListOfDatesForChalets.FindAsync(id);
            if (listOfDatesForChalet != null)
            {
                _context.ListOfDatesForChalets.Remove(listOfDatesForChalet);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("YourChalet", "AccountInfo");
        }

    }
}
