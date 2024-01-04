using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rehleti.EmailService;
using Rehleti.Models;

namespace Rehleti.Controllers
{
    public class BookTripsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IEmailSender _emailSender;
        public BookTripsController(ModelContext context,IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }


        // GET: BookTrips/Create
        public async Task <IActionResult> Create(int id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            HttpContext.Session.SetInt32("TripID",id);
            ViewData["AdventureTripId"] = new SelectList(_context.AdventureTrips, "Id", "Id");
            ViewData["ChaletId"] = new SelectList(_context.Chalets, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword");

            var AdventureTrip=await _context.AdventureTrips.Where(x=>x.Id== HttpContext.Session.GetInt32("TripID") && x.NumberOfGuests>0 && x.DateFrom>DateTime.Now).FirstOrDefaultAsync();
            ViewData["ImgTripPath"] = AdventureTrip.ImagePath;
            var user = await _context.UserGuests.Where(x => x.Id == HttpContext.Session.GetInt32("UserID")).AsNoTracking().FirstOrDefaultAsync();
            var book = await _context.BookTrips.ToListAsync();
            if (AdventureTrip==null)
            {
                return NotFound();
            }
            var all = Tuple.Create<AdventureTrip, UserGuest, IEnumerable<BookTrip>>(AdventureTrip,user,book);
            return View(all);
        }

        // POST: BookTrips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,AdventureTripId")] BookTrip bookTrip, int AdventureTripID, int NumberOfAdventurous)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetInt32("UserID") == null)
                {
                    return RedirectToAction("SignIn", "Account", new { area = "" });
                }
                bookTrip.UserId = HttpContext.Session.GetInt32("UserID");
                bookTrip.AdventureTripId = AdventureTripID;
                var trip = await _context.AdventureTrips.Where(x => x.Id == AdventureTripID).AsNoTracking().FirstOrDefaultAsync();
                var user = await _context.UserGuests.Where(x => x.Id == HttpContext.Session.GetInt32("UserID")).AsNoTracking().FirstOrDefaultAsync();

                if (trip.NumberOfGuests < NumberOfAdventurous)
                {
                    return View("ErrorNumberOfAdventurous","BookTrips");
                }

                trip.NumberOfGuests -= NumberOfAdventurous;
                var newBook = new BookTrip
                {
                    UserId = user.Id,
                    AdventureTripId = trip.Id
                };
                _context.Add(newBook);
                _context.Update(trip);
                await _context.SaveChangesAsync();

               

                await _emailSender.SendEmailAsync(user.Email, "Confirmation of Your Adventure Trip Booking with Rehleti",
               "Dear " + user.FirstName + " " + user.LastName +
               ",\r\n\r\nI hope this email finds you well. We are delighted to inform you that your booking for an adventurous trip" +
               " through Rehleti has been successfully confirmed. We appreciate your trust in our platform to plan your adventure, and we" +
               " are committed to ensuring a memorable and exciting experience for you and your group.Here are the details of your adventure" +
               " trip: \r\n\r\n Trip Check-In Date: " + trip.DateFrom + "\r\nTrip Check-Out Date " + trip.DateTo +
               "Number Of Adventurous : " + NumberOfAdventurous + "\r\n" + "\r\n\r\nPlease double - check the provided information to ensure its accuracy." +
               "If you notice any discrepancies or have specific requests, please do not hesitate to reach out to our customer support team.\r\n\r\n Payment Information: \r\n" +
               "Kindly note that the payment for your adventure trip is to be handled directly between you and the adventure company.Rehleti" +
               " is a platform for booking, and we do not process payments for the trips. Upon confirmation of your booking, you should" +
               " receive payment instructions and details from the adventure company.If you encounter any issues or require assistance" +
               " with payments, we recommend contacting the adventure company directly.\r\n\r\n Travel Documents: \r\n In preparation for" +
               " your adventure, make sure to have all the necessary travel documents, such as identification, permits, and any other" +
               " requirements specified by the adventure company.We also recommend reviewing the company's policies and guidelines for" +
               " a smooth and enjoyable experience.\r\n\r\n Emergency Contact:\r\nFor any emergencies or last - minute changes to your" +
               " itinerary, please keep the adventure company's contact information handy. They will be your primary point of contact" +
               " during the trip.We would like to take this opportunity to thank you for choosing Rehleti for your adventure trip.We" +
               " look forward to hearing about your thrilling experiences upon your return.If you have any questions or require further" +
               " assistance, please do not hesitate to contact our customer support team.Safe travels and best wishes for an unforgettable" +
               " adventure!\r\n\r\nWarm regards,\r\n\r\nRehleti Customer Support Team");

                var owner=await _context.UserGuests.Where(x=>x.Id==trip.CompanyOwnerId).AsNoTracking().FirstOrDefaultAsync();

                await _emailSender.SendEmailAsync(owner.Email, "Booking Confirmation for Adventure Trip",
               "Dear " + owner.FirstName + " " + owner.LastName +
               ",\r\n\r\nI hope this email finds you well. We are pleased to inform you that one of our esteemed users has booked an" +
               " adventure trip with your company through our website." +
               "\r\nHere are the details of the booking:\r\n\r\nUser Information:\r\n\r\nUser's Name: " + user.FirstName + " " + user.LastName +
               "\r\nUser's Email Address: " + user.Email + "\r\nUser's Contact Number: " + user.PhoneNumber + "\r\n" +
               "Number Of adventurous" + NumberOfAdventurous + "\r\n\r\nAdventure Trip Details:\r\n\r\nTrip Name: " + trip.Name
               + "\r\nCheck-In Date of Trip: " + trip.DateFrom + "\r\nCheck-Out Date of Trip: " + trip.DateTo + " \r\n" +
               "Our user is looking forward to a thrilling and memorable adventure experience with your company. We have provided them" +
               " with all the necessary information regarding the trip and the booking process. As a trusted platform, we have facilitated" +
               " the booking, and the user is excited to embark on this adventure.\r\n\r\nWe kindly request that you ensure a seamless and" +
               " enjoyable experience for our user and their fellow adventurers. If there are any specific requirements or details that ou" +
               "r user should be aware of, please do not hesitate to communicate with them directly. We believe that your expertise and" +
               " dedication to providing exceptional adventure trips will make this experience truly unforgettable.\r\n\r\nAs the user's" +
               " point of contact, we trust that you will handle all aspects of the trip professionally, ensuring their safety, enjoyment," +
               " and satisfaction. We greatly value our partnership with your company and look forward to continued successful" +
               " collaborations in the future.\r\n\r\nIf you require any further information or have any specific requests" +
               " related to this booking, please do not hesitate to reach out to us. We are here to assist in any way we can to make this" +
               " adventure trip a resounding success.\r\n\r\nThank you for choosing Rehleti for your online booking needs. We are confident" +
               " that your adventure services will leave a lasting impression on our user.\r\n\r\nBest regards,\r\n\r\nRehleti Website\r\n");


                return RedirectToAction(nameof(Index),"Home");
            }

            ViewData["AdventureTripId"] = new SelectList(_context.AdventureTrips, "Id", "Id", bookTrip.AdventureTripId);
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", bookTrip.UserId);
            return View(bookTrip);
        }

        // GET: BookTrips/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.BookTrips == null)
            {
                return NotFound();
            }

            var bookTrip = await _context.BookTrips.FindAsync(id);
            if (bookTrip == null)
            {
                return NotFound();
            }
            ViewData["AdventureTripId"] = new SelectList(_context.AdventureTrips, "Id", "Id", bookTrip.AdventureTripId);
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", bookTrip.UserId);
            return View(bookTrip);
        }

        // POST: BookTrips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,UserId,ChaletId,AdventureTripId,IsBook")] BookTrip bookTrip)
        {
            if (id != bookTrip.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookTrip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookTripExists(bookTrip.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdventureTripId"] = new SelectList(_context.AdventureTrips, "Id", "Id", bookTrip.AdventureTripId);
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", bookTrip.UserId);
            return View(bookTrip);
        }

        // GET: BookTrips/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.BookTrips == null)
            {
                return NotFound();
            }

            var bookTrip = await _context.BookTrips
                .Include(b => b.AdventureTrip)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookTrip == null)
            {
                return NotFound();
            }

            return View(bookTrip);
        }

        // POST: BookTrips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (_context.BookTrips == null)
            {
                return Problem("Entity set 'ModelContext.BookTrips'  is null.");
            }
            var bookTrip = await _context.BookTrips.FindAsync(id);
            if (bookTrip != null)
            {
                _context.BookTrips.Remove(bookTrip);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ErrorNumberOfAdventurous()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            return View();
        }
        private bool BookTripExists(decimal id)
        {
          return (_context.BookTrips?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}