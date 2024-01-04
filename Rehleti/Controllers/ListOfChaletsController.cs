using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;
using System.Runtime.InteropServices;

namespace Rehleti.Controllers
{
    public class ListOfChaletsController : Controller
    {
        private readonly ModelContext _context;
        public ListOfChaletsController(ModelContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            var chalets = await _context.Chalets
                .Where(x => x.Status == "Approved")
                .OrderByDescending(x => x.Rating)
                .AsNoTracking()
                .ToListAsync();

            foreach (var item in chalets)
            {
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    // Split the paths using the semicolon delimiter
                    string[] pathsArray = item.ImagePath.Split(';');
                    // Check if there are any paths after splitting
                    if (pathsArray.Length > 0)
                    {
                        // Store the first path in the ImagePath property
                        item.ImagePath = pathsArray[0].Trim(); // Store the first path and trim any extra spaces
                    }
                }
            }

            return View(chalets);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime? checkIn, DateTime? checkOut, int? numberOfGuests)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            var chalets = await _context.Chalets.AsNoTracking().ToListAsync();

            if (checkIn != null && checkOut != null && numberOfGuests != null)
            {
                chalets = await _context.Chalets
                        .Where(x => x.NumberOfGuests >= numberOfGuests &&
                            _context.ListOfDatesForChalets.Any(d =>
                                d.ChaletId == x.Id &&
                                (d.DateFrom == checkIn &&
                                d.DateTo == checkOut)))
                        .AsNoTracking()
                        .ToListAsync();
            }
            else if (checkIn != null && checkOut != null && numberOfGuests == null)
            {
                chalets = await _context.Chalets.Where(x => x.NumberOfGuests > 0 &&
                                                _context.ListOfDatesForChalets.Any(d => d.ChaletId == x.Id &&
                                                                d.DateFrom == checkIn &&
                                                                d.DateTo == checkOut)).AsNoTracking().ToListAsync();
            }
            else if (checkIn != null && checkOut == null && numberOfGuests == null)
            {
                chalets = await _context.Chalets.Where(x => x.NumberOfGuests > 0 &&
                                                _context.ListOfDatesForChalets.Any(d => d.ChaletId == x.Id &&
                                                                d.DateFrom == checkIn)).AsNoTracking().ToListAsync();
            }
            else if (checkIn != null && checkOut == null && numberOfGuests != null)
            {
                chalets = await _context.Chalets.Where(x => x.NumberOfGuests >= numberOfGuests &&
                                                 _context.ListOfDatesForChalets.Any(d => d.ChaletId == x.Id &&
                                                                 d.DateFrom == checkIn)).AsNoTracking().ToListAsync();
            }
            else if (checkIn != null && checkOut == null && numberOfGuests == null)
            {
                chalets = await _context.Chalets.Where(x => x.NumberOfGuests > 0 &&
                                                 _context.ListOfDatesForChalets.Any(d => d.ChaletId == x.Id &&
                                                                 d.DateFrom == checkIn)).AsNoTracking().ToListAsync();
            }
            else if (checkIn == null && checkOut == null && numberOfGuests != null)
            {
                chalets = await _context.Chalets.Where(x => x.NumberOfGuests >= numberOfGuests).AsNoTracking().ToListAsync();
            }
            else if (checkIn == null && checkOut != null && numberOfGuests != null)
            {
                chalets = await _context.Chalets.Where(x => x.NumberOfGuests >= numberOfGuests &&
                                                 _context.ListOfDatesForChalets.Any(d => d.ChaletId == x.Id &&
                                                                 d.DateTo == checkOut)).AsNoTracking().ToListAsync();
            }
            else if (checkIn == null && checkOut != null && numberOfGuests == null)
            {
                chalets = await _context.Chalets.Where(x => x.NumberOfGuests > 0 &&
                                                 _context.ListOfDatesForChalets.Any(d => d.ChaletId == x.Id &&
                                                                 d.DateTo == checkOut)).AsNoTracking().ToListAsync();
            }
            foreach (var item in chalets)
            {
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    // Split the paths using the semicolon delimiter
                    string[] pathsArray = item.ImagePath.Split(';');
                    // Check if there are any paths after splitting
                    if (pathsArray.Length > 0)
                    {
                        // Store the first path in the ImagePath property
                        item.ImagePath = pathsArray[0].Trim(); // Store the first path and trim any extra spaces
                    }
                }
            }
            return View(chalets);
        }
    }
}