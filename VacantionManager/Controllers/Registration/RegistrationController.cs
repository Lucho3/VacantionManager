using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models.Entity;
using VacantionManager.Models;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace VacantionManager.Controllers.Registration
{
    public class RegistrationController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(VacantionManagerDBContext context, ILogger<RegistrationController> logger)
        {
            _logger = logger;
            _context = context;
        }

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

        // POST: Register user
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("username,firstName,lastName,password,confirmPassword")] UserModel userModel)
        {         
            if (ModelState.IsValid)
            {
                if (!_context.Users.Select(u => u.username).Contains(userModel.username))
                {
                    userModel.role = await _context.Roles.Where(r => r.name == "Unassigned").FirstOrDefaultAsync();
                    userModel.password =Utilities.HashFunctions.HashPassword(userModel.password);
                    _context.Add(userModel);
                    await _context.SaveChangesAsync();
                    return View("Index","LogIn");
                }
                else
                {
                    ViewData["Error message"] = "This username is taken!";
                    return View();
                }
            }
            else 
            { 
                    ViewData["Error message"] = "One or more field has data that doesn't match the criteria for it!";
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
