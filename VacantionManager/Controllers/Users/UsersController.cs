using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacantionManager.Models;
using VacantionManager.Models.Entity;

namespace VacantionManager.Controllers.Users

{
    public class UsersController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<UsersController> _logger;

        private UserModel user = null;
        public UsersController(VacantionManagerDBContext context, ILogger<UsersController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET: UserModels
        public async Task<IActionResult> Index()
        {
            if (await extractUser())
            {
                List<UserModel> users = await _context.Users.Include(u => u.role).ToListAsync();
                if (user.role.name == "CEO")
                {
                    return View(users);
                }
                else
                {
                    return View("IndexNormalUser", users);
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }
        // GET: UserModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (await extractUser())
            {
                UserModel userModel = await _context.Users.Include(u => u.role).Include(u => u.team)
                    .FirstOrDefaultAsync(m => m.id == id);
                if (user.role.name == "CEO")
                {
                    if (id == null)
                    {
                        return NotFound();
                    }


                    if (userModel == null)
                    {
                        return NotFound();
                    }

                    return View(userModel);
                }
                else
                {
                    return View("DetailsNormalUser", userModel);
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        // GET: UserModels/Create
        public async Task<IActionResult> Create()
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    return View();
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

        // POST: UserModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,username,firstName,lastName,password")] UserModel userModel)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (ModelState.IsValid)
                    {
                        _context.Add(userModel);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    return View(userModel);
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

        // GET: UserModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var userModel = await _context.Users.FindAsync(id);
                    if (userModel == null)
                    {
                        return NotFound();
                    }
                    return View(userModel);
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

        // POST: UserModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,username")] UserModel userModel)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (id != userModel.id)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(userModel);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!UserModelExists(userModel.id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    return View(userModel);
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

        // GET: UserModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var userModel = await _context.Users
                        .FirstOrDefaultAsync(m => m.id == id);
                    if (userModel == null)
                    {
                        return NotFound();
                    }

                    return View(userModel);
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

        // POST: UserModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    var userModel = await _context.Users.FindAsync(id);
                    _context.Users.Remove(userModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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

        private bool UserModelExists(int id)
        {
            return _context.Users.Any(e => e.id == id);
        }

        public async Task<IActionResult> Search(string search)
        {
            List<UserModel> users = await _context.Users.Include(u => u.role).ToListAsync();
            if (await extractUser())
            {
                if (search != null)
                {
                    string searchBy = Request.Form["SearchBy"];
                    if (searchBy == "role")
                    {
                        users = await _context.Users
                                              .Include(u => u.role)
                                              .Include(u => u.team)
                                              .Where(u => u.role.name
                                              .Contains(search)).ToListAsync();

                        ViewData["Message"] = "Searched by role!";
                        return viewByTypeUser(users);
                    }
                    else if (searchBy == "username")
                    {
                        users = await _context.Users
                                              .Include(u => u.role)
                                              .Include(u => u.team)
                                              .Where(u => u.username
                                              .Contains(search)).ToListAsync();

                        ViewData["Message"] = "Searched by username!";
                        return viewByTypeUser(users);
                    }
                    else if (searchBy == "firstName")
                    {
                        users = await _context.Users
                                              .Include(u => u.role)
                                              .Include(u => u.team)
                                              .Where(u => u.firstName
                                              .Contains(search)).ToListAsync();

                        ViewData["Message"] = "Searched by first name!";
                        return viewByTypeUser(users);
                    }
                    else
                    {
                        users = await _context.Users
                                              .Include(u => u.role)
                                              .Include(u => u.team)
                                              .Where(u => u.lastName
                                              .Contains(search)).ToListAsync();

                        ViewData["Message"] = "Searched by last name!";
                        return viewByTypeUser(users);
                    }
                }
                else
                {
                    ViewData["Message"] = "You must type in searchbox!";
                    return viewByTypeUser(users);
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
        private async Task<bool> extractUser()
        {
            byte[] buffer = new byte[200];
            if (HttpContext.Session.TryGetValue("id", out buffer))
            {
                int userId = int.Parse(Encoding.UTF8.GetString(buffer));
                user = await _context.Users.Include(u => u.role).FirstOrDefaultAsync(u => u.id == userId);
                return true;
            }
            else
            {
                return false;
            }
        }


        //I MUST USE THIS EVERYWHERE
        [NonAction]
        private  ViewResult viewByTypeUser(object model)
        {
            if (user.role.name == "CEO")
            {
                return View("Index", model);
            }
            else
            {
                return View("IndexNormalUser", model);
            }
        }
    }
}
