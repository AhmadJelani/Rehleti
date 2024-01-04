using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Asn1.X509;
using Rehleti.EmailService;
using Rehleti.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rehleti.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class AdventureTripsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IEmailSender _emailSender;
        public AdventureTripsController(ModelContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Dashboard/AdventureTrips
        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            var modelContext = _context.AdventureTrips.Include(a => a.CompanyOwner);
            return View(await modelContext.ToListAsync());
        }

        // GET: Dashboard/AdventureTrips/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
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

        // GET: Dashboard/AdventureTrips/Edit/5
        public async Task<IActionResult> Approve(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.AdventureTrips == null)
            {
                return NotFound();
            }

            var adventureTrip = await _context.AdventureTrips.FindAsync(id);
            if (adventureTrip == null)
            {
                return NotFound();
            }
            ViewData["CompanyOwnerId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", adventureTrip.CompanyOwnerId);
            return View(adventureTrip);
        }

        // POST: Dashboard/AdventureTrips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(decimal id, [Bind("Id,Name,Description,ProofOfCompanyOwner,JoinDate,NumberOfGuests,Price,CompanyOwnerId,Status,DateTo,DateFrom,ImagePath")] AdventureTrip adventureTrip)
        {
            if (id != adventureTrip.Id)
            {
                return NotFound();
            }
            try
            {
                adventureTrip.Status = "Approved";
                _context.Update(adventureTrip);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdventureTripExists(adventureTrip.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var user = await _context.UserGuests.FindAsync(adventureTrip.CompanyOwnerId);
            await _emailSender.SendEmailAsync(user.Email, "Request to List Your adventure trip on Rehleti",
                "Dear " + user.FirstName + " " + user.LastName + ",\r\n\r\nI hope this email finds you well. We appreciate your interest in listing" +
                " your adventure trip on Rehleti, the premier website for booking chalets and adventure trips in Jordan." +
                " Our mission is to promote and motivate domestic tourism in this beautiful country," +
                " and we are thrilled to have you as a part of our growing community.\r\n\r\nI am pleased to" +
                " inform you that your adventure trip has been carefully reviewed, and we are impressed with the quality and" +
                " unique offerings it presents. Therefore, I am delighted to approve your request to have your adventure trip" +
                " listed on the Rehleti website.\r\n\r\nYour participation in our platform not only aligns with our core" +
                " values but also contributes to the diversification of accommodation options for our users, making their" +
                " experiences in Jordan truly memorable. Your adventure trip will be a valuable addition to the range of adventure" +
                " trips and lodging options we offer.\r\n\r\nBefore we proceed with the listing, please ensure that you" +
                " have all the necessary information, details, and high-quality photo of your adventure trip ready for the listing" +
                " process. Our team will provide you with guidance and support during this phase to ensure that your adventure trip" +
                " is presented in the best possible way to attract potential guests.\r\n\r\nOnce your adventure trip is live on our" +
                " website, you can expect an increase in visibility, bookings, and a greater connection with travelers" +
                " seeking unique and authentic experiences in Jordan.\r\n\r\nIf you have any questions or require further" +
                " assistance during the listing process, please do not hesitate to reach out to our dedicated support team." +
                " They are here to help you every step of the way.\r\n\r\nThank you for choosing Rehleti to showcase your" +
                " adventure trip, and we look forward to a successful partnership that benefits both your business and the domestic" +
                " tourism industry in Jordan.\r\n\r\nWelcome aboard, and best wishes for a prosperous and fulfilling journey" +
                " on Rehleti!\r\n\r\nSincerely,\r\n\r\nRehleti - Booking chalets and Adventure" +
                " Trips in Jordan\r\n");
            return RedirectToAction(nameof(Index));
        }

        private bool AdventureTripExists(decimal id)
        {
          return (_context.AdventureTrips?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        // GET: Dashboard/AdventureTrips/Edit/5
        public async Task<IActionResult> Reject(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.AdventureTrips == null)
            {
                return NotFound();
            }

            var adventureTrip = await _context.AdventureTrips.FindAsync(id);
            if (adventureTrip == null)
            {
                return NotFound();
            }
            ViewData["CompanyOwnerId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", adventureTrip.CompanyOwnerId);
            return View(adventureTrip);
        }

        // POST: Dashboard/AdventureTrips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(decimal id, [Bind("Id,Name,Description,ProofOfCompanyOwner,JoinDate,NumberOfGuests,Price,CompanyOwnerId,Status,DateTo,DateFrom,ImagePath")] AdventureTrip adventureTrip)
        {
            if (id != adventureTrip.Id)
            {
                return NotFound();
            }
            try
            {
                adventureTrip.Status = "Rejected";
                _context.Update(adventureTrip);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdventureTripExists(adventureTrip.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var user = await _context.UserGuests.FindAsync(adventureTrip.CompanyOwnerId);
            await _emailSender.SendEmailAsync(user.Email, "Request to List Your adventure trip on Rehleti",
            "Dear " + user.FirstName + " " + user.LastName + "\r\n\r\nI hope this message finds you well. We would like to" +
                " express our sincere gratitude for your interest in listing your adventure trip on Rehleti, the premier platform" +
                " for booking chalets and adventure trips in Jordan, with a mission to promote domestic tourism.After" +
                " careful consideration of your request, we regret to inform you that we are unable to proceed with the" +
                " listing of your adventure trip on our platform at this time.Our decision was not made lightly, and we appreciate" +
                " the effort you put into your application.Please understand that our team has rigorous criteria and" +
                " standards for the properties listed on Rehleti to ensure an exceptional experience for our users." +
                " Unfortunately, your adventure trip did not meet all the necessary requirements to be featured on our platform." +
                "We understand the time and effort you have invested in this process, and we genuinely appreciate your" +
                " interest in collaborating with us.While your adventure trip may not be eligible for listing at this time, please" +
                " consider that our platform may evolve in the future, and we encourage you to reapply once you meet our" +
                " listing criteria.We are committed to supporting the domestic tourism industry in Jordan and constantly" +
                " seek to enhance the experiences we offer to our users.We wish you the best of luck in your endeavors," +
                " and we hope that you will consider Rehleti in the future when the opportunity arises.Thank you for your" +
                " understanding and your interest in Rehleti.If you have any questions or need further clarification" +
                " regarding our decision, please do not hesitate to reach out to us.Once again, we appreciate your" +
                " interest and hope to see you succeed in your venture.\r\n\r\nSincerely,\r\n\r\nRehleti - Booking" +
                " Chalets and Adventure" +
                " Trips in Jordan\r\n");
            return RedirectToAction(nameof(Index));
        }
    }
}