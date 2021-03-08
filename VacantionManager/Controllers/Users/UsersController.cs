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

namespace VacantionManager.Controllers.Users
{
    public class UsersController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<UsersController> _logger;

        private List<UserModel> users = null;

        private string userRole = null;

        private int userId = 0;
        public UsersController(VacantionManagerDBContext context, ILogger<UsersController> logger)
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

                if (users == null)
                {
                    return NotFound();
                }

                return  ViewReturner(users);
            }

            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> Search(string search)
        {
            if (await extractIdAndUser())
            {
                if (search != null)
                {
                    string searchBy = Request.Form["SearchBy"];
                    if (searchBy=="role")
                    {
                        users = await _context.Users
                                              .Include(u => u.role)
                                              .Include(u=>u.team)
                                              .Where(u => u.role.name
                                              .Contains(search)).ToListAsync();

                        ViewData["Message"] = "Searched by role!";
                        return View("NormalUsers",users);
                    }
                    else if (searchBy == "username")
                    {
                        users = await _context.Users
                                              .Include(u => u.role)
                                              .Include(u=>u.team)
                                              .Where(u => u.username
                                              .Contains(search)).ToListAsync();

                        ViewData["Message"] = "Searched by username!";
                        return View("NormalUsers", users);
                    }
                    else if (searchBy == "firstName")
                    {
                        users = await _context.Users
                                              .Include(u => u.role)
                                              .Include(u => u.team)
                                              .Where(u => u.firstName
                                              .Contains(search)).ToListAsync();

                        ViewData["Message"] = "Searched by first name!";
                        return View("NormalUsers", users);
                    }
                    else
                    {
                        users = await _context.Users
                                              .Include(u => u.role)
                                              .Include(u => u.team)
                                              .Where(u => u.lastName
                                              .Contains(search)).ToListAsync();

                        ViewData["Message"] = "Searched by last name!";
                        return View("NormalUsers", users);
                    }
                }
                else
                {
                    ViewBag["Message"] = "You must type in searchbox!";
                    return View(users);
                }                
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (await extractIdAndUser())
            {
                if (userRole == "CEO")
                {
                    return View("CEOCreateUser");
                }
                else
                {
                    return View("NoPermission");
                }
                
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("username,firstName,lastName,password,confirmPassword")] UserModel userModel)
        {
            if(await extractIdAndUser())
            {
                if(userRole == "CEO")
                {
                    userModel.confirmPassword = userModel.password.ToString();
                    if (ModelState.IsValid)
                    {
                        if (!_context.Users.Select(u => u.username).Contains(userModel.username))
                        {
                            userModel.role = await _context.Roles.Where(r => r.name == "Unassigned").FirstOrDefaultAsync();
                            userModel.password = Utilities.HashFunctions.HashPassword(userModel.password);
                            _context.Add(userModel);
                            await _context.SaveChangesAsync();
                            users.Add(userModel);
                            return  ViewReturner(users);
                        }
                        else
                        {
                            ViewData["Error message"] = "This username is taken!";
                            return View("CEOCreateUser");
                        }
                    }
                    else
                    {
                        ViewData["Error message"] = "One or more field has data that doesn't match the criteria for it!";
                        return View("CEOCreateUser");
                    }
                }
                else
                {
                    return View("NoPermission");
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (await extractIdAndUser())
            {
                 if (id == null)
                 {
                     return NotFound();
                 }

                 UserModel userModel = await _context.Users.Include(u=>u.role).Include(u=>u.team)
                     .FirstOrDefaultAsync(m => m.id == id);
                 if (userModel == null)
                 {
                     return NotFound();
                 }

                 return View("~/Views/Users/UserDetails.cshtml",userModel);
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
                UserModel u = await _context.Users.Include(u => u.role).FirstOrDefaultAsync(u => u.id == userId);
                userRole = u.role.name;
                users = await _context.Users.Include(u=>u.team).Include(u => u.role).ToListAsync();

               // foreach (UserModel user in usersDb)
               // {
               //     users.Add(new UserViewModel { id = user.id, username = user.username, firstName = user.firstName, lastName = user.lastName });
               // }
                return true;
            }
            else
            {
                return false;
            }
        }

        [NonAction]
        private ViewResult ViewReturner(object model)
        {           
           if (userRole== "CEO")
           {
               return View("CEOUsers",model);
           }
           else
           {
              return View("NormalUsers", model);
           }
        }
    }
}
