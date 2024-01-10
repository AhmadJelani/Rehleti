using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Rehleti.Models;

namespace Rehleti.Controllers
{
    public class ChaletsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _environment;

        public ChaletsController(ModelContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Chalets
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
            return View(await modelContext.ToListAsync());
        }

        // GET: Chalets/Details/5
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

            if (chalet == null)
            {
                return NotFound();
            }
            else {
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
            }
            return View(chalet);
        }

        // GET: Chalets/Create
        public IActionResult Create()
        {
            ViewData["ID"] = HttpContext.Session.GetInt32("UserID");
            ViewData["Name"] = HttpContext.Session.GetString("FirstName") + " " + HttpContext.Session.GetString("LastName");
            ViewData["ImagePath"] = HttpContext.Session.GetString("ImagePath");
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("SignIn", "Account", new { area = "" });
            }
            ViewData["OwnerId"] = new SelectList(_context.UserGuests, "Id", "Id");
            return View();
        }

        // POST: Chalets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ProofOfOwner,Rating,JoinDate,NumberOfGuests,Price,LacationLatitude,OwnerId,ImagePath,LacationLongitude,Status,ImageFile,PDFFile")] Chalet chalet, string locationLatitude, string locationLongitude, List<IFormFile> ImagesFile)
        {
            if (ImagesFile != null && ImagesFile.Any())
            {
                List<string> savedPaths = new List<string>();
                foreach (var item in ImagesFile)
                {
                    if (!(item.ContentType == "image/jpeg" || item.ContentType == "image/jpg" || item.ContentType == "image/png"))
                    {
                        ViewData["ImageError"] = "Make sure to add the photo with the appropriate extensions (jpg, jpeg, or png).";
                        return View(chalet);
                    }
                    string wwwRootPath = _environment.WebRootPath;
                    string file_Name = Guid.NewGuid().ToString() + item.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", file_Name);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(fileStream);
                    }
                    // Save the relative path (after the "/Images/") to the list
                    string relativePath = path.Replace(wwwRootPath + "/Images/", "");
                    savedPaths.Add(relativePath);
                }
                // Convert the list of relative paths to a single string
                string concatenatedPaths = string.Join(";", savedPaths);
                // Set the concatenated image paths to the chalet.ImagePath property
                chalet.ImagePath = concatenatedPaths;
            }

            if (chalet.PDFFile != null && chalet.PDFFile.Length > 0)
            {
                if (chalet.PDFFile.ContentType != "application/pdf")
                {
                    ViewData["FilePDFError"]= "Make sure to add the PDF file";
                    return View(chalet);
                }
                else
                {
                    string webRootPath = _environment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(chalet.PDFFile.FileName);

                    string path = Path.Combine(webRootPath + "/Proof Files/", fileName);

                    try
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await chalet.PDFFile.CopyToAsync(fileStream);
                        }
                        chalet.ProofOfOwner = fileName;
                    }
                    catch (IOException ex)
                    {
                        ViewData["ErrorFile"] = ex.Message;
                    }
                }
            }
            chalet.LacationLongitude = locationLongitude;
            chalet.LacationLatitude = locationLatitude;
            chalet.Status = "Pending";
            chalet.JoinDate = DateTime.Now;
            chalet.OwnerId = HttpContext.Session.GetInt32("UserID");
            chalet.Rating = 0;

            _context.Add(chalet);
            await _context.SaveChangesAsync();

            ViewData["OwnerId"] = new SelectList(_context.UserGuests, "Id", "Id", chalet.OwnerId);
            return RedirectToAction("YourChalet", "AccountInfo");
        }
    }
}