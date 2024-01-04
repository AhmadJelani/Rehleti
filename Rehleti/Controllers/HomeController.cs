using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;
using System.Collections;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Rehleti.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;

        public HomeController(ILogger<HomeController> logger, ModelContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            var users = await _context.UserGuests.AsNoTracking().ToListAsync();
            var chaletsFeedbacks = await _context.ChaletFeedbacks.AsNoTracking().ToListAsync();
            var tripsFeedback = await _context.TripFeedbacks.AsNoTracking().ToListAsync();
            var cha = await _context.Chalets.AsNoTracking().ToListAsync();
            var trip = await _context.AdventureTrips.AsNoTracking().ToListAsync();

            var chaletJoin = from f in chaletsFeedbacks
                             join u in users on f.UserId equals u.Id
                             join c in cha on f.ChaletId equals c.Id
                             select new FeedbackChaletJoinTable
                             {
                                 user = u,
                                 feedback = f,
                                 chalet = c
                             };
            var tripJoin = from f in tripsFeedback
                           join u in users on f.UserId equals u.Id
                           join t in trip on f.TripId equals t.Id
                           select new FeedbackTripJoinTable
                           {
                               user = u,
                               feedback = f,
                               trip = t
                           };

            var dates = await _context.ListOfDatesForChalets
                .Where(x => x.Status == "Unbooked" && x.DateFrom.Value.Date > DateTime.Now)
                .AsNoTracking()
                .ToListAsync();

            var chalet = await _context.Chalets
               .Where(x => x.Status == "Approved")
               .OrderByDescending(x => x.Rating)
               .AsNoTracking()
               .Take(3)
               .ToListAsync();

            var AdvenTrips = await _context.AdventureTrips
                .Where(x => x.Status == "Approved" && x.NumberOfGuests > 0 && x.DateFrom > DateTime.Now)
                .AsNoTracking()
                .Take(3)
                .ToListAsync();

            foreach (var item in chalet)
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
            var all = Tuple.Create<IEnumerable<Chalet>, IEnumerable<AdventureTrip>, IEnumerable<FeedbackChaletJoinTable>, IEnumerable<FeedbackTripJoinTable>>
                (chalet, AdvenTrips, chaletJoin, tripJoin);

            return View(all);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> AboutUs()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            var NumberOfChalets = await _context.Chalets.Where(x => x.Status == "Approved").CountAsync();
            var NumberOfAdventureTrips = await _context.AdventureTrips.Where(x => x.Status == "Approved").CountAsync();
            var numberOfUsers = await _context.UserGuests.Where(x => x.RoleId == 22).CountAsync();
            if (NumberOfChalets == null || NumberOfAdventureTrips == null || numberOfUsers == null)
            {
                return View("Error");
            }
            var all = Tuple.Create(NumberOfChalets, NumberOfAdventureTrips, numberOfUsers);
            return View(all);
        }
        public async Task<IActionResult> ContactUs()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            return View();
        }

    }
}