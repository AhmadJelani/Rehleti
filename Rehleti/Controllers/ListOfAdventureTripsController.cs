using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;

namespace Rehleti.Controllers
{
    public class ListOfAdventureTripsController : Controller
    {
        private readonly ModelContext _context;
        public ListOfAdventureTripsController(ModelContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            var trip = await _context.AdventureTrips
                .Where(x => x.Status == "Approved" && x.NumberOfGuests > 0 && x.DateFrom > DateTime.Now)
                .AsNoTracking()
                .ToListAsync();

            return View(trip);
        }
        [HttpPost]
        public async Task<IActionResult> Index(DateTime? checkIn, DateTime? checkOut, int? numberOfGuests)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            var trip = await _context.AdventureTrips
                .Where(x => x.Status == "Approved" && x.NumberOfGuests > 0 && x.DateFrom > DateTime.Now)
                .AsNoTracking()
                .ToListAsync();

            if (checkIn != null && checkOut != null && numberOfGuests != null)
            {
                return View(trip.Where(x =>
                    x.DateFrom == checkIn && x.DateTo == checkOut && x.NumberOfGuests >= numberOfGuests));
            }
            else if (checkIn != null && checkOut != null && numberOfGuests == null)
            {
                return View(trip.Where(x =>
                    x.DateFrom == checkIn && x.DateTo == checkOut));
            }
            else if (checkIn != null && checkOut == null && numberOfGuests == null)
            {
                return View(trip.Where(x => x.DateFrom == checkIn));
            }
            else if (checkIn == null && checkOut == null && numberOfGuests == null)
            {
                return View(trip);
            }
            else if (checkIn == null && checkOut == null && numberOfGuests != null)
            {
                return View(trip.Where(x => x.NumberOfGuests >= numberOfGuests));
            }
            else if (checkIn == null && checkOut != null && numberOfGuests != null)
            {
                return View(trip.Where(x => x.DateTo == checkOut && x.NumberOfGuests >= numberOfGuests));
            }

            return View(trip);
        }

    }
}
