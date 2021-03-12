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

namespace VacantionManager.Controllers.Roles
{
    public class RolesController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<RolesController> _logger;

        private UserModel user=null;

        public RolesController(VacantionManagerDBContext context, ILogger<RolesController> logger)
        {
            _context = context;
            _logger =logger;
        }

        // GET: RoleModels
        public async Task<IActionResult> Index()
        {
            if (await extractUser())
            {
                List<RoleModel> roles = await _context.Roles.Include(r => r.users).ToListAsync();
                if (user.role.name == "CEO")
                {
                    return View(roles);
                }
                else
                {
                    return View("IndexNormalUser",roles );
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }           
        }

        // GET: RoleModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (await extractUser())
            {
                var roleModel = await _context.Roles.Include(r => r.users)
                                        .FirstOrDefaultAsync(m => m.id == id);
                if (user.role.name == "CEO")
                {
                    if (id == null)
                    {
                        return NotFound();
                    }


                    if (roleModel == null)
                    {
                        return NotFound();
                    }

                    return View(roleModel);
                }
                else
                {
                    return View("DetailsNormalUser", roleModel);
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        // GET: RoleModels/Create
        public async Task<IActionResult> Create()
        {
            if (await extractUser())
            {
                if (user.role.name=="CEO")
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

        // POST: RoleModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name")] RoleModel roleModel)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (ModelState.IsValid)
                    {
                        _context.Add(roleModel);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    return View(roleModel);
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

        // GET: RoleModels/Edit/5
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

                      var roleModel = await _context.Roles.FindAsync(id);
                      if (roleModel == null)
                      {
                          return NotFound();
                      }
                      return View(roleModel);
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

        // POST: RoleModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name")] RoleModel roleModel)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (id != roleModel.id)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(roleModel);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!RoleModelExists(roleModel.id))
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
                    return View(roleModel);
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

        // GET: RoleModels/Delete/5
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

                      var roleModel = await _context.Roles
                          .FirstOrDefaultAsync(m => m.id == id);
                      if (roleModel == null)
                      {
                          return NotFound();
                      }

                      return View(roleModel);
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

        // POST: RoleModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                     var roleModel = await _context.Roles.FindAsync(id);
                     _context.Roles.Remove(roleModel);
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

        private bool RoleModelExists(int id)
        {
            return _context.Roles.Any(e => e.id == id);
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

