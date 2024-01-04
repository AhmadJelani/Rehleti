using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;
using System.Runtime.InteropServices;

namespace Rehleti.Controllers
{

    public class AccountInfoController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;
        public AccountInfoController(ModelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Info(int? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (id == null || _context.UserGuests == null)
            {
                return NotFound();
            }
            var userGuest = await _context.UserGuests
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userGuest == null)
            {
                return NotFound();
            }

            return View(userGuest);
        }
        // GET: Dashboard/UserGuests/Edit/5
        public async Task<IActionResult> EditAccount(decimal? id)
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            if (id == null || _context.UserGuests == null)
            {
                return NotFound();
            }

            var userGuest = await _context.UserGuests.FindAsync(id);
            if (userGuest == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userGuest.RoleId);
            return View(userGuest);
        }

        // POST: Dashboard/UserGuests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAccount(decimal id, [Bind("Id,FirstName,LastName,Email,Password,ConfirmPassword,PhoneNumber,JoinDate,RoleId,ImagePath,ImageFile")] UserGuest userGuest)
        {
            if (id != userGuest.Id)
            {
                return NotFound();
            }
            try
            {
            if (userGuest.ImageFile != null)
            {
                    if (!(userGuest.ImageFile.ContentType == "image/jpeg" || userGuest.ImageFile.ContentType == "image/jpg" || userGuest.ImageFile.ContentType == "image/png"))
                    {
                        ViewData["ImgError"] = "Make sure to add the photo with the appropriate extensions (jpg, jpeg, or png).";
                        return View(userGuest);
                    }
                string wwwRootPath = AppDomain.CurrentDomain.BaseDirectory;

                string File_Name = Guid.NewGuid().ToString() + userGuest.ImageFile.FileName;

                string path = Path.Combine(wwwRootPath + "/Images/" + File_Name);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await userGuest.ImageFile.CopyToAsync(fileStream);
                }
                userGuest.ImagePath = File_Name;
            }
            if (userGuest.ConfirmPassword != userGuest.Password)
            {
                    ViewData["WrongPass"] = "Make sure that password and confirm passwordare the same";
                    return View(userGuest);
            }
            userGuest.ConfirmPassword = userGuest.Password;
            _context.Update(userGuest);
            await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserGuestExists(userGuest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index),"Home");
        }
        private bool UserGuestExists(decimal id)
        {
            return (_context.UserGuests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<IActionResult> YourChalet() {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var chalet = await _context.Chalets.Where(x => x.OwnerId == HttpContext.Session.GetInt32("UserID")).ToListAsync();
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

            return View(chalet);
        }
        public async Task<IActionResult> YourAdventureTrip()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            return View(await _context.AdventureTrips.Where(x => x.CompanyOwnerId == HttpContext.Session.GetInt32("UserID")).ToListAsync());
        }
    } 
}