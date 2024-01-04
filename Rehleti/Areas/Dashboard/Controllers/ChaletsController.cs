using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rehleti.Models;
using MailKit.Net.Smtp;
using Rehleti.EmailService;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.CodeAnalysis;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Bcpg.Sig;
using Org.BouncyCastle.Math;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Composition;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Rehleti.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class ChaletsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IEmailSender _emailSender;
        public ChaletsController(ModelContext context,IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }
        // GET: Dashboard/Chalets
        public async Task<IActionResult> Index()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            var modelContext = _context.Chalets.Include(c => c.Owner);
            foreach (var item in modelContext)
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
            return View(await modelContext.ToListAsync());
        }
        // GET: Dashboard/Chalets/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.Chalets == null)
            {
                return NotFound();
            }

            var chalet = await _context.Chalets
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (!string.IsNullOrEmpty(chalet.ImagePath))
            {
                // Split the paths using the semicolon delimiter
                string[] pathsArray = chalet.ImagePath.Split(';');
                // Check if there are any paths after splitting
                if (pathsArray.Length > 0)
                {
                    // Store the first path in the ImagePath property
                    chalet.ImagePath = pathsArray[0].Trim(); // Store the first path and trim any extra spaces
                }
            }
            if (chalet == null)
            {
                return NotFound();
            }

            return View(chalet);
        }
        // GET: Dashboard/Chalets/Edit/5
        public async Task<IActionResult> Approve(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.Chalets == null)
            {
                return NotFound();
            }

            var chalet = await _context.Chalets.FindAsync(id);
            if (chalet == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", chalet.OwnerId);
            return View(chalet);
        }
        // POST: Dashboard/Chalets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(decimal id, [Bind("Id,Name,Description,ProofOfOwner,Rating,JoinDate,NumberOfGuests,Price,LacationLongitude,LacationLatitude,Status,OwnerId,ImagePath")] Chalet chalet)
        {
            if (id != chalet.Id)
            {
                return NotFound();
            }
            try
            {
                chalet.Status = "Approved";
                _context.Update(chalet);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChaletExists(chalet.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var user = await _context.UserGuests.FindAsync(chalet.OwnerId);
            await _emailSender.SendEmailAsync(user.Email, "Request to List Your Chalet on Rehleti",
                "Dear "+user.FirstName+" "+user.LastName+",\r\n\r\nI hope this email finds you well. We appreciate your interest in listing" +
                " your chalet on Rehleti, the premier website for booking chalets and adventure trips in Jordan." +
                " Our mission is to promote and motivate domestic tourism in this beautiful country," +
                " and we are thrilled to have you as a part of our growing community.\r\n\r\nI am pleased to" +
                " inform you that your chalet has been carefully reviewed, and we are impressed with the quality and" +
                " unique offerings it presents. Therefore, I am delighted to approve your request to have your chalet" +
                " listed on the Rehleti website.\r\n\r\nYour participation in our platform not only aligns with our core" +
                " values but also contributes to the diversification of accommodation options for our users, making their" +
                " experiences in Jordan truly memorable. Your chalet will be a valuable addition to the range of adventure" +
                " trips and lodging options we offer.\r\n\r\nBefore we proceed with the listing, please ensure that you" +
                " have all the necessary information, details, and high-quality photos of your chalet ready for the listing" +
                " process. Our team will provide you with guidance and support during this phase to ensure that your chalet" +
                " is presented in the best possible way to attract potential guests.\r\n\r\nOnce your chalet is live on our" +
                " website, you can expect an increase in visibility, bookings, and a greater connection with travelers" +
                " seeking unique and authentic experiences in Jordan.\r\n\r\nIf you have any questions or require further" +
                " assistance during the listing process, please do not hesitate to reach out to our dedicated support team." +
                " They are here to help you every step of the way.\r\n\r\nThank you for choosing Rehleti to showcase your" +
                " chalet, and we look forward to a successful partnership that benefits both your business and the domestic" +
                " tourism industry in Jordan.\r\n\r\nWelcome aboard, and best wishes for a prosperous and fulfilling journey" +
                " on Rehleti!\r\n\r\nSincerely,\r\n\r\nRehleti - Booking Chalets and Adventure" +
                " Trips in Jordan\r\n");
            return RedirectToAction(nameof(Index));
        }
        private bool ChaletExists(decimal id)
        {
          return (_context.Chalets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        // GET: Dashboard/Chalets/Edit/5
        public async Task<IActionResult> Reject(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            if (id == null || _context.Chalets == null)
            {
                return NotFound();
            }

            var chalet = await _context.Chalets.FindAsync(id);
            if (chalet == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", chalet.OwnerId);
            return View(chalet);
        }

        // POST: Dashboard/Chalets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(decimal id, [Bind("Id,Name,Description,ProofOfOwner,Rating,JoinDate,NumberOfGuests,Price,LacationLongitude,LacationLatitude,Status,OwnerId,ImagePath")] Chalet chalet)
        {
            if (id != chalet.Id)
            {
                return NotFound();
            }
            try
            {
                chalet.Status = "Rejected";
                _context.Update(chalet);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChaletExists(chalet.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var user = await _context.UserGuests.FindAsync(chalet.OwnerId);

            await _emailSender.SendEmailAsync(user.Email, "Request to List Your Chalet on Rehleti",
                "Dear " + user.FirstName + " " + user.LastName + "I hope this message finds you well. We would like to" +
                " express our sincere gratitude for your interest in listing your chalet on Rehleti, the premier platform" +
                " for booking chalets and adventure trips in Jordan, with a mission to promote domestic tourism.After" +
                " careful consideration of your request, we regret to inform you that we are unable to proceed with the" +
                " listing of your chalet on our platform at this time.Our decision was not made lightly, and we appreciate" +
                " the effort you put into your application.Please understand that our team has rigorous criteria and" +
                " standards for the properties listed on Rehleti to ensure an exceptional experience for our users." +
                " Unfortunately, your chalet did not meet all the necessary requirements to be featured on our platform." +
                "We understand the time and effort you have invested in this process, and we genuinely appreciate your" +
                " interest in collaborating with us.While your chalet may not be eligible for listing at this time, please" +
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