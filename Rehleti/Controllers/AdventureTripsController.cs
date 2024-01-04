using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Rehleti.Models;

namespace Rehleti.Controllers
{
    public class AdventureTripsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;
        public AdventureTripsController(ModelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: AdventureTrips
        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var modelContext = _context.AdventureTrips.Include(a => a.CompanyOwner);
            return View(await modelContext.ToListAsync());
        }

        // GET: AdventureTrips/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (id == null || _context.AdventureTrips == null)
            {
                return NotFound();
            }
            var adventureTrip = await _context.AdventureTrips
                .Include(a => a.CompanyOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureTrip == null)
            {
                return NotFound();
            }
            return View(adventureTrip);
        }
        // GET: AdventureTrips/Create
        public IActionResult Create()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            ViewData["CompanyOwnerId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword");
            return View();
        }
        // POST: AdventureTrips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ProofOfCompanyOwner,JoinDate,NumberOfGuests,Price,CompanyOwnerId,DateTo,DateFrom,ImagePath,PDFFile,ImageFile")] AdventureTrip adventureTrip)
        {
            if (ModelState.IsValid)
            {
                if (adventureTrip.ImageFile != null)
                {
                    string wwwRootPath = AppDomain.CurrentDomain.BaseDirectory;
                    string File_Name = Guid.NewGuid().ToString() + adventureTrip.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/" + File_Name);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await adventureTrip.ImageFile.CopyToAsync(fileStream);
                    }
                    adventureTrip.ImagePath = File_Name;
                }
                if (adventureTrip.PDFFile.ContentType != "application/pdf")
                {
                    ModelState.AddModelError("PDFFile", "Only PDF files are allowed.");
                }
                else
                {
                    if (adventureTrip.PDFFile != null && adventureTrip.PDFFile.Length > 0)
                    {
                        string webRootPath = _environment.WebRootPath;
                        string fileName = Guid.NewGuid() + Path.GetExtension(adventureTrip.PDFFile.FileName);// Use a unique name to prevent overwriting existing files.
                        string path = Path.Combine(webRootPath + "/Proof Files/" + fileName);
                        try
                        {
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await adventureTrip.PDFFile.CopyToAsync(fileStream);
                            }
                            adventureTrip.ProofOfCompanyOwner = fileName;
                        }
                        catch (IOException ex)
                        {
                            ViewData["Error File"] = ex.Message;
                        }
                    }
                }
                adventureTrip.Status = "Pending";
                adventureTrip.JoinDate = DateTime.Now;
                adventureTrip.CompanyOwnerId = HttpContext.Session.GetInt32("UserID");

                _context.Add(adventureTrip);
                await _context.SaveChangesAsync();
                return RedirectToAction("YourAdventureTrip","AccountInfo");
            }
            ViewData["CompanyOwnerId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", adventureTrip.CompanyOwnerId);
            return View(adventureTrip);
        }
        // GET: AdventureTrips/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (id == null || _context.AdventureTrips == null)
            {
                return NotFound();
            }

            var adventureTrip = await _context.AdventureTrips
                .Include(a => a.CompanyOwner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adventureTrip == null)
            {
                return NotFound();
            }

            return View(adventureTrip);
        }

        // POST: AdventureTrips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.AdventureTrips == null)
            {
                return Problem("Entity set 'ModelContext.AdventureTrips'  is null.");
            }
            var adventureTrip = await _context.AdventureTrips.FindAsync(id);
            if (adventureTrip != null)
            {
                _context.AdventureTrips.Remove(adventureTrip);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("YourAdventureTrip","AccountInfo");
        }

        private bool AdventureTripExists(decimal id)
        {
          return (_context.AdventureTrips?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
