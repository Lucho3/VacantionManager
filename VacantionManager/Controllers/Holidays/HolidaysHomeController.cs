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

namespace VacantionManager.Controllers.Holidays
{
    public class HolidaysHomeController : Controller
    {

        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<HolidaysHomeController> _logger;

        private UserModel user = null;

        public HolidaysHomeController(VacantionManagerDBContext context, ILogger<HolidaysHomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            if (await extractUser())
            {
                return View("~/Views/Holidays/HolidaysHome.cshtml");
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [NonAction]
        private async Task<bool> extractUser()
        {
            byte[] buffer = new byte[200];
            if (HttpContext.Session.TryGetValue("id", out buffer))
            {
                int userId = int.Parse(Encoding.UTF8.GetString(buffer));
                user = await _context.Users.Where(u => u.id == userId).Include(u => u.role).FirstOrDefaultAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
