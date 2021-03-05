using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacantionManager.Models.Entity;
using VacantionManager.Models;

namespace VacantionManager.Controllers.Registration
{
    public class RegistrationController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        public RegistrationController(VacantionManagerDBContext context)
        {           
            _context = context;
        }

        public IActionResult Index()
        {           
            return View();
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
                    userModel.role = _context.Roles.Where(r => r.name == "Unassigned").FirstOrDefault();
                    userModel.password = Utilities.HashFunctions.HashPassword(userModel.password);
                    _context.Add(userModel);
                    await _context.SaveChangesAsync();
                    return View();
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
    }
}
