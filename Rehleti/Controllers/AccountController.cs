using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rehleti.Models;

namespace Rehleti.Controllers
{
    public class AccountController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;
        public AccountController(ModelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Register()
        {
            return View();
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register([Bind("Id,FirstName,LastName,Email,Password,ConfirmPassword,PhoneNumber,ImagePath,ImageFile")] UserGuest userGuest)
		{
			if (ModelState.IsValid)
			{
				if (userGuest.ImageFile != null)
				{
                    string wwwRootPath = AppDomain.CurrentDomain.BaseDirectory;

					string File_Name = Guid.NewGuid().ToString() + userGuest.ImageFile.FileName;

					string path = Path.Combine(wwwRootPath + "/Images/" + File_Name);

					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						await userGuest.ImageFile.CopyToAsync(fileStream);
					}
					userGuest.ImagePath = File_Name;
				}
				var user=await _context.UserGuests.Where(g => g.Email == userGuest.Email).AsNoTracking().SingleOrDefaultAsync();
                if (user == null) {
                    userGuest.JoinDate = DateTime.Now;
                    userGuest.RoleId = 22;
                    if (userGuest.Password == userGuest.ConfirmPassword)
                    {
                        _context.Add(userGuest);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(SignIn),"Account");
                    }
                    else {
                        ViewData["PasswordError"] = "Please check the password";
					}
				}
                else {
                    ViewData["UsedEmail"] = "Email is used";
                }
				
			}
			ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", userGuest.RoleId);
			return View(userGuest);
		}
		public IActionResult SignIn()
        {
            return View();
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SignIn([Bind("Id,Email,Password")] UserGuest userGuest)
		{
			var ValidUser =await _context.UserGuests.Where(x => x.Email == userGuest.Email && x.Password == userGuest.Password).FirstOrDefaultAsync();
			if (ValidUser != null)
			{
				switch (ValidUser.RoleId)
				{
					case 1:
						HttpContext.Session.SetInt32("UserID", (int)ValidUser.Id);
						HttpContext.Session.SetString("FirstName", ValidUser.FirstName);
						HttpContext.Session.SetString("LastName", ValidUser.LastName);
						HttpContext.Session.SetString("Email", ValidUser.Email);
						HttpContext.Session.SetString("Password",ValidUser.Password);
						HttpContext.Session.SetString("ImagePath",ValidUser.ImagePath);
						return RedirectToAction("Index", "Home", new { area = "Dashboard" });
					case 22:
						HttpContext.Session.SetInt32("UserID", (int)ValidUser.Id);
						HttpContext.Session.SetString("FirstName", ValidUser.FirstName);
						HttpContext.Session.SetString("LastName", ValidUser.LastName);
						HttpContext.Session.SetString("Email", ValidUser.Email);
                        HttpContext.Session.SetString("Password", ValidUser.Password);
                        HttpContext.Session.SetString("ImagePath", ValidUser.ImagePath);
                        return RedirectToAction("Index", "Home");
				}
			}
			else
			{
				ViewData["ErrorSignIn"] = "Invalid Email Or Password";
			}
			return View(userGuest);
		}
        public async Task <IActionResult> Logout()
        {
			HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
		public IActionResult ForgotPassword() {
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ForgotPassword([Bind("Id,Email,Password,ConfirmPassword")] UserGuest userGuest)
		{
            if (ModelState.IsValid)
            {
                var user = await _context.UserGuests.FirstOrDefaultAsync(x => x.Email == userGuest.Email);
                if (user != null)
                {
                    if (userGuest.Password == userGuest.ConfirmPassword)
                    {
                        try
                        {
                            user.Password = userGuest.Password;
                            user.ConfirmPassword = userGuest.ConfirmPassword;
                            _context.Update(user);
                            await _context.SaveChangesAsync();
                            return View(nameof(SignIn));
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
                    }
                    else
                    {
                        ViewData["PasswordError"] = "Passwords do not match. Please check and try again.";
                        return View(userGuest);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            return View(userGuest);
        }
        private bool UserGuestExists(decimal id)
        {
            return (_context.UserGuests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}