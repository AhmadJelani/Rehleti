using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rehleti.Models;
using Stripe;
using Stripe.Checkout;

namespace Rehleti.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ModelContext _context;
        public PaymentController(ModelContext context)
        {
            _context = context;
        }//https://localhost:7167/Payment/SuccessVisaCard
        [HttpPost]
        public async Task<IActionResult> VisaCardChalet(int chaletID,DateTime Date)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }

            var chalet = await _context.Chalets
                .Where(x => x.Id == chaletID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (chalet == null)
            {
                return NotFound();
            }
            else {

                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>{
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)((float)chalet.Price * 100), // Amount in cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = chalet.Name,
                                Description = chalet.Description,
                            }
                        }
                    }
                    },
                    Mode = "payment",
                    SuccessUrl = "http://rehleti.com/Payment/SuccessVisaCard",
                    CancelUrl = "http://rehleti.com/Payment/CancelVisaCard"
                };
                var service = new SessionService();
                var session = service.Create(options);
                // Redirect to the Stripe Checkout session URL
                return Redirect(session.Url);
            }
        }
        public IActionResult SuccessVisaCard() 
        {
            return View();
        }
        public IActionResult CancelVisaCard()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VisaCardTrip(int TripID,int NumberOfAdventurous)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }

            var trip = await _context.AdventureTrips
                .Where(x => x.Id == TripID)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (trip == null)
            {
                return NotFound();
            }
            else
            {

                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>{
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)((float)trip.Price * 100), // Amount in cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = trip.Name,
                                Description = trip.Description,
                            }
                        }
                    }
                    },
                    Mode = "payment",
                    SuccessUrl = "http://rehleti.com/Payment/SuccessVisaCard",
                    CancelUrl = "http://rehleti.com/Payment/CancelVisaCard"
                };
                var service = new SessionService();
                var session = service.Create(options);
                // Redirect to the Stripe Checkout session URL
                trip.NumberOfGuests -= NumberOfAdventurous;
                return Redirect(session.Url);
            }
        }
    }
}
