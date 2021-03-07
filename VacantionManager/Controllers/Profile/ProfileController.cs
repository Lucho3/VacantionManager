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
using VacantionManager.Models.HelperModels;
using VacantionManager.Models.Entity;

namespace VacantionManager.Controllers.Profile
{
    public class ProfileController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<ProfileController> _logger;

        private UserModel user = null;

        private int userId = 0;
        public ProfileController(VacantionManagerDBContext context, ILogger<ProfileController> logger)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (await extractIdAndUser())
            {
                if (userId <= 0)
                {
                    return NotFound();
                }

                if (user == null)
                {
                    return NotFound();
                }

                return await userView(user);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> ChangeUsername(string username)
        {          
            if (await extractIdAndUser())
            {
                if (!String.IsNullOrEmpty(username))
                {
                    if (!_context.Users.Select(u => u.username).Contains(username))
                    {
                        user.username = username;
                        await _context.SaveChangesAsync();
                        return await userView(user);
                    }
                    else
                    {
                        ViewData["Message"]= "This username is taken or it is the same!";
                        return await userView(user);
                    }
                }
                else
                {
                    ViewData["Message"]= "Username is required!";
                    return await userView(user);
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> ChangePassword(string oldPassword,string newPassword,string confirmPassword)
        {
            if (await extractIdAndUser())
            {
                if (!String.IsNullOrEmpty(newPassword) && !String.IsNullOrEmpty(oldPassword) && !String.IsNullOrEmpty(confirmPassword))
                {
                    if (Utilities.UserValidation.passwordCheck(newPassword))
                    {
                        if (Utilities.HashFunctions.ComparePasswords(user.password, oldPassword))
                        {
                            if (newPassword == confirmPassword)
                            {
                                user.password = Utilities.HashFunctions.HashPassword(newPassword);
                                await _context.SaveChangesAsync();
                                ViewData["Message"] = "Changed successfully!";
                                return await userView(user);
                            }
                            else
                            {
                                ViewData["Message"] = "new password and confirmed passwords doesn't match!";
                                return await userView(user);
                            }

                        }
                        else
                        {
                            ViewData["Message"] = "The old password doesn't match!";
                            return await userView(user);
                        }
                    }
                    else
                    {
                        ViewData["Message"] = "New password doesn't match the criteria!";
                        return await userView(user);
                    }
                }
                else
                {
                    ViewData["Message"] = "One or more fields are empty!";
                    return await userView(user);
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }


        public async Task<IActionResult> ChangeFirstName(string firstName)
        {
            if (await extractIdAndUser())
            {
                if (!String.IsNullOrEmpty(firstName))
                {
                    user.firstName = firstName;
                    await _context.SaveChangesAsync();
                    return await userView(user);
                }
                else
                {
                    ViewData["Message"]= "First name is required!";
                    return await userView(user);
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> ChangeLastName(string lastName)
        {
            if (await extractIdAndUser())
            {
                if (!String.IsNullOrEmpty(lastName))
                {
                    user.lastName = lastName;
                    await _context.SaveChangesAsync();
                    return await userView(user);
                }
                else
                {
                    ViewData["Message"]= "First name is required!";
                    return await userView(user);
                }
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
        private async Task<bool> extractIdAndUser()
        {
            byte[] buffer = new byte[200];
            if (HttpContext.Session.TryGetValue("id", out buffer))
            {
                userId = int.Parse(Encoding.UTF8.GetString(buffer));
                user = await _context.Users.Include(u => u.role).Include(u => u.team)
               .FirstOrDefaultAsync(m => m.id == userId);
                return true;
            }
            else
            {
                return false;
            }
        }

        [NonAction]
        private async Task<ViewResult> userView(object model)
        {
            if ( user.role.name == "CEO")
            {
                List<string> roles = await _context.Roles.Select(r => r.name).ToListAsync();
                List<string> teams = await _context.Teams.Select(t => t.name).ToListAsync();
                return View("IndexSuperiorUser",new UserProfileModel(model,roles,teams));
            }
            else
            {
                return View("IndexNormalUser",model);
            }
        }


    }
}
