using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacantionManager.Models;
using VacantionManager.Models.Entity;

namespace VacantionManager.Controllers.LogIn
{
    public class LogInController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<LogInController> _logger;

        public IActionResult Index()
        {
            byte[] buffer = new byte[200];
            if (!HttpContext.Session.TryGetValue("id", out buffer))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
       
        public LogInController(VacantionManagerDBContext context, ILogger<LogInController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // POST: Register user
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string username, string password)
        {
            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
            {
                UserModel user = await _context.Users.FirstOrDefaultAsync(u => u.username == username);
                if (user!=null)
                {
                    string hashedPassword = Utilities.HashFunctions.HashPassword(password);
                    if (Utilities.HashFunctions.CompareHashedPasswords(user.password,hashedPassword))
                    {
                        HttpContext.Session.Set("id", Encoding.UTF8.GetBytes(user.id.ToString()));
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewData["Error message"] = "Wrong password!";
                        return View();
                    }
                }
                else
                {
                    ViewData["Error message"] = "There is no such user!";
                    return View();
                }
            }
            else
            {
                ViewData["Error message"] = "One of the fields is empty!";
                return View();
            }                       
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
