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

namespace VacantionManager.Controllers.Home

{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly VacantionManagerDBContext _context;

        private UserModel user;

        private int userId=0;

        public HomeController(VacantionManagerDBContext context, ILogger<HomeController> logger)
        {           
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            byte[] buffer = new byte[200];
            if (HttpContext.Session.TryGetValue("id", out buffer))
            {
                userId = int.Parse(Encoding.UTF8.GetString(buffer));
                user = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);
                ViewData["Full name"] = user.firstName + " " + user.lastName;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","LogIn");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
