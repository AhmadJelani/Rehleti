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
    public class BookChaletsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IEmailSender _emailSender;
        public BookChaletsController(ModelContext context,IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: BookChalets/Create
        public async Task <IActionResult> Create(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");

            ViewData["ChaletId"] = new SelectList(_context.Chalets, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword");

            var user=await _context.UserGuests.Where(x=>x.Id== HttpContext.Session.GetInt32("UserID")).AsNoTracking().FirstOrDefaultAsync();
            var chalet = await _context.Chalets.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
            var date=await _context.ListOfDatesForChalets.Where(x=>x.ChaletId==id && x.Status== "Unbooked" && x.DateFrom>DateTime.Now).ToListAsync();
            var book=await _context.BookChalets.ToListAsync();

            var users = await _context.UserGuests.AsNoTracking().ToListAsync();
            var chaletsFeedbacks = await _context.ChaletFeedbacks.Where(x=>x.ChaletId==id).AsNoTracking().ToListAsync();
            var cha = await _context.Chalets.AsNoTracking().ToListAsync();

            var chaletJoin = from f in chaletsFeedbacks
                             join u in users on f.UserId equals u.Id
                             join c in cha on f.ChaletId equals c.Id
                             select new FeedbackChaletJoinTable
                             {
                                 user = u,
                                 feedback = f,
                                 chalet = c
                             };


            List<string> imagePathsList = new List<string>();

            if (!string.IsNullOrEmpty(chalet.ImagePath))
            {
                // Split the paths using the semicolon delimiter
                string[] pathsArray = chalet.ImagePath.Split(';');

                // Loop through the array and add each item to the list
                foreach (string path in pathsArray)
                {
                    // Trim any extra spaces and add to the list
                    imagePathsList.Add(path.Trim());
                }
            }
            var all = Tuple.Create<UserGuest,Chalet,IEnumerable<ListOfDatesForChalet>,IEnumerable<BookChalet>,List<string>, IEnumerable<FeedbackChaletJoinTable>>(user,chalet,date,book,imagePathsList, chaletJoin);
            return View(all);
        }

        // POST: BookChalets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id,[Bind("Id,ChaletId,UserId,DateId")] BookChalet bookChalet, DateTime Date)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Session.GetInt32("UserID") == null)
                {
                    return RedirectToAction("SignIn", "Account", new { area = "" });
                }
                bookChalet.ChaletId= id;
                bookChalet.UserId = HttpContext.Session.GetInt32("UserID");
                var checkIn = await _context.ListOfDatesForChalets.Where(x => x.DateFrom == Date).FirstOrDefaultAsync();
                if (checkIn==null)
                {
                    return NotFound();
                }
                bookChalet.DateId=checkIn.Id;
                checkIn.Status = "Booked";
                var book = new BookChalet
                {
                    ChaletId = bookChalet.ChaletId,
                    UserId = bookChalet.UserId,
                    DateId = bookChalet.DateId
                };
                _context.Update(checkIn);
                _context.Add(book);
                await _context.SaveChangesAsync();

                var user = await _context.UserGuests.Where(x => x.Id == HttpContext.Session.GetInt32("UserID")).AsNoTracking().FirstOrDefaultAsync();
                var chalet=await _context.Chalets.Where(x=>x.Id==bookChalet.Id).AsNoTracking().FirstOrDefaultAsync();
                var date = await _context.ListOfDatesForChalets.Where(x => x.Id == bookChalet.DateId).AsNoTracking().FirstOrDefaultAsync();
                var owner = await _context.UserGuests.Where(x => x.Id == chalet.OwnerId).AsNoTracking().FirstOrDefaultAsync();

                await _emailSender.SendEmailAsync(user.Email, "Confirmation of Your Chalet Booking with Rehleti",
               "Dear " + user.FirstName + " " + user.LastName +
               ",\r\n\r\nWe hope this email finds you well. We are delighted to confirm your recent chalet booking through the Rehleti website" +
               " for your upcoming adventure trip. Your booking details are as follows:\r\n\r\nChalet Name: "+chalet.Name+"\r\nCheck-In" +
               " Date: "+date.DateFrom+ "\r\nCheck-Out Date: " +date.DateTo + "\r\nNumber of Guests: "+chalet.NumberOfGuests+ "\r\n\r\nPlease note that" +
               " the payment for your chalet reservation is to be settled directly with the owner of the chalet. Rehleti website serves as" +
               " a platform to connect travelers with property owners and facilitate bookings, but we do not handle payments. You will" +
               " receive contact information for the chalet owner shortly.\r\n\r\nOwner Information:\r\nOwner's Name: "+ owner.FirstName+" " +owner.LastName+
               "\r\nEmail: "+owner.Email+ "\r\nPhone: "+owner.PhoneNumber+ "\r\n\r\nIf you have any questions or need to" +
               " make any adjustments to your booking, please do not hesitate to contact us at [Rehleti Customer Support Email] or call" +
               " us at [Rehleti Customer Support Phone Number].\r\n\r\nIt's important to note that your payment for this booking will not" +
               " be processed through the Rehleti website. Instead, the payment arrangements will be coordinated directly between you and" +
               " the chalet company. You will receive instructions on how to complete the payment from the chalet company separately." +
               "\r\n\r\nAs you prepare for your adventure, we recommend that you reach out to the chalet company directly to confirm your" +
               " booking details, ask any questions you may have, and coordinate any additional services or requests.\r\n\r\nYour" +
               " satisfaction and enjoyment of your chalet experience are of the utmost importance to us. If you encounter any issues or" +
               " require assistance during your stay, please don't hesitate to reach out to our dedicated customer support team, and we" +
               " will do our best to assist you.\r\n\r\nThank you for choosing Rehleti for your chalet booking needs. We appreciate your" +
               " trust in our platform, and we are confident that you will have an unforgettable adventure.\r\n\r\nSafe travels, and may" +
               " your stay at the chalet be filled with joy, relaxation, and new memories to cherish.\r\n\r\nWarm regards,\r\n\r\n" +
               "\r\nCustomer Service Team\r\nRehleti Website");


                await _emailSender.SendEmailAsync(owner.Email, "Booking Confirmation for Your Chalet with Rehleti",
               "Dear " + owner.FirstName + " " + owner.LastName +
               ",\r\n\r\nI hope this message finds you well. We are pleased to inform you that a user of Rehleti website has recently" +
               " made a reservation for your chalet through our platform. Rehleti is committed to facilitating seamless bookings for both chalet" +
               " owners and our valued users. Please find below the details of the reservation:\r\n\r\nUser Information:\r\n\r\nUser's" +
               " Name: "+user.FirstName+" "+user.LastName+ "\r\nContact Email: "+user.Email+ "\r\nContact Phone Number: "+user.PhoneNumber+ "\r\nChalet" +
               " Information:\r\n\r\nChalet Name: "+chalet.Name+ "\r\nReservation Dates: "+date.DateFrom+ " to "+date.DateTo+ "\r\nNumber of" +
               " Guests: "+chalet.NumberOfGuests+ "\r\nTotal Price: "+chalet.Price+ "\r\nSpecial Requests: [Any Special Requests or Notes]\r\nWe kindly" +
               " remind you that, per our platform policy, the payment arrangements are made directly between the user and the chalet owner." +
               " Therefore, please contact the user directly to coordinate payment details and any specific terms or conditions related to their stay." +
               " We recommend reaching out to the user at your earliest convenience to ensure a smooth and enjoyable experience during their stay at" +
               " your chalet.\r\n\r\nShould you have any questions or require assistance with the reservation, please do not hesitate to reach out to" +
               " our support team at [Rehleti Support Email] or [Rehleti Support Phone Number].\r\n\r\nWe appreciate your partnership and trust in" +
               " Rehleti to assist you in connecting with guests. If you have any feedback or suggestions for improvement, we welcome your insights" +
               " as we continually strive to enhance the booking experience for both our chalet owners and users.\r\n\r\nThank you for choosing" +
               " Rehleti for your chalet booking needs. We look forward to more successful collaborations in the future.\r\n\r\nBest regards," +
               "\r\n\r\nRehleti Website");
                return RedirectToAction(nameof(Index),"Home");
            }
            ViewData["ChaletId"] = new SelectList(_context.Chalets, "Id", "Id", bookChalet.ChaletId);
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", bookChalet.UserId);
            return View(bookChalet);
        }

        // GET: BookChalets/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.BookChalets == null)
            {
                return NotFound();
            }

            var bookChalet = await _context.BookChalets.FindAsync(id);
            if (bookChalet == null)
            {
                return NotFound();
            }
            ViewData["ChaletId"] = new SelectList(_context.Chalets, "Id", "Id", bookChalet.ChaletId);
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", bookChalet.UserId);
            return View(bookChalet);
        }

        // POST: BookChalets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,ChaletId,UserId")] BookChalet bookChalet)
        {
            if (id != bookChalet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookChalet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookChaletExists(bookChalet.Id))
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
            ViewData["ChaletId"] = new SelectList(_context.Chalets, "Id", "Id", bookChalet.ChaletId);
            ViewData["UserId"] = new SelectList(_context.UserGuests, "Id", "ConfirmPassword", bookChalet.UserId);
            return View(bookChalet);
        }

        // GET: BookChalets/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.BookChalets == null)
            {
                return NotFound();
            }

            var bookChalet = await _context.BookChalets
                .Include(b => b.Chalet)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookChalet == null)
            {
                return NotFound();
            }

            return View(bookChalet);
        }

        // POST: BookChalets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.BookChalets == null)
            {
                return Problem("Entity set 'ModelContext.BookChalets'  is null.");
            }
            var bookChalet = await _context.BookChalets.FindAsync(id);
            if (bookChalet != null)
            {
                _context.BookChalets.Remove(bookChalet);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookChaletExists(decimal id)
        {
          return (_context.BookChalets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
