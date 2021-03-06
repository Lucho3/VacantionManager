using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models;
using VacantionManager.Models.Entity;

namespace VacantionManager.Controllers.LogIn
{
    public class LogInController : Controller
    {
        private readonly VacantionManagerDBContext _context;
        public IActionResult Index()
        {
            return View();
        }
       
        public LogInController(VacantionManagerDBContext context)
        {
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
                        ViewData["Error message"] = "Logged";
                        return View();
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
    }
}
