using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;

namespace Rehleti.Controllers
{
    public class MyTripsController : Controller
    {
        private readonly ModelContext _context;
        public MyTripsController(ModelContext context)
        {
            _context = context;
        }
        public async Task <IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            var bookChalet = await _context.BookChalets.Where(x => x.UserId == HttpContext.Session.GetInt32("UserID")).AsNoTracking().ToListAsync();
            var bookTrip = await _context.BookTrips.Where(x=>x.UserId== HttpContext.Session.GetInt32("UserID")).AsNoTracking().ToListAsync();
            var chalet=await _context.Chalets.AsNoTracking().ToListAsync();
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
            var trip=await _context.AdventureTrips.AsNoTracking().ToListAsync();

            var testChalet = from bc in bookChalet
                             join c in chalet on bc.ChaletId equals c.Id into temp
                             from t in temp.DefaultIfEmpty()
                             select new MyTripsJoinTableChalet
                             {
                                 bookChalet = bc,
                                 chalet = t
                             };

            var counter = testChalet.Count();


            var testTrip   = from bt in bookTrip
                             join t in trip on bt.AdventureTripId equals t.Id
                             select new MyTripsJoinTablesTrip
                             {
                                 bookTrip = bt,
                                 trip = t
                             };


            var both = Tuple.Create<IEnumerable<MyTripsJoinTableChalet>,IEnumerable<MyTripsJoinTablesTrip>>(testChalet,testTrip);
            return View(both);
        }
    }
}
