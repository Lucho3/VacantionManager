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
using VacantionManager.Models.ViewModels;

namespace VacantionManager.Controllers.Teams
{
    public class TeamsController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<TeamsController> _logger;

        private UserModel user = null;

        public TeamsController(VacantionManagerDBContext context, ILogger<TeamsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: TeamModels
        public async Task<IActionResult> Index()
        {
            if (await extractUser())
            {
                List<TeamModel> teams = await _context.Teams.Include(t => t.teamLeader).Include(t => t.devs.Where(u=>u.role.name!= "Unassigned")).ToListAsync();
                if (user.role.name == "CEO")
                {
                    return View(teams);
                }
                else
                {
                    return View("IndexNormalUser",teams);
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }           
        }

        // GET: TeamModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamModel = await _context.Teams
                .FirstOrDefaultAsync(m => m.id == id);
            if (teamModel == null)
            {
                return NotFound();
            }

            return View(teamModel);
        }

        // GET: TeamModels/Create
        public async Task<IActionResult> Create()
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    //TODO: different method
                    Dictionary<string, string> users = await _context.Users.Include(u => u.role).Where(u => u.role.name == "Team Lead" && u.team == null).ToDictionaryAsync(u => u.username, u => (u.firstName + " " + u.lastName));
                    List<string> projects = await _context.Projects.Select(p => p.name).ToListAsync();
                    TeamViewModel twm = new TeamViewModel(users, projects);
                    return View(twm);
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

        // POST: TeamModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string teamName)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    //TODO: different method
                    Dictionary<string, string> users = await _context.Users.Include(u => u.role).Where(u => u.role.name == "Team Lead" && u.team == null).ToDictionaryAsync(u => u.username, u => (u.firstName + " " + u.lastName));
                    List<string> projects = await _context.Projects.Select(p => p.name).ToListAsync();
                    TeamViewModel twm = new TeamViewModel(users, projects);
                    if (teamName != null)
                    {
                        string project = Request.Form["Projects"];
                        string teamLead = Request.Form["TeamLeads"];
                        UserModel leader = await _context.Users.Where(u => u.username == teamLead).FirstOrDefaultAsync();
                        TeamModel temMod = new TeamModel
                        {
                            name = teamName,
                        };
                        
                        if (project != null)
                        {
                            temMod.project = await _context.Projects.Where(p => p.name == project).FirstOrDefaultAsync();

                        }
                        if (teamLead != null)
                        {
                            temMod.teamLeader = await _context.Users.Where(u => u.username == teamLead).FirstOrDefaultAsync();
                            leader.team = temMod;
                            leader.leadedTeam = temMod;
                        }
                       
                        _context.Add(temMod);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ViewData["Message"] = "You must chose name of the team!";
                        return View(twm);
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

        // GET: TeamModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamModel = await _context.Teams.FindAsync(id);
            if (teamModel == null)
            {
                return NotFound();
            }
            return View(teamModel);
        }

        // POST: TeamModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name")] TeamModel teamModel)
        {
            if (id != teamModel.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teamModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamModelExists(teamModel.id))
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
            return View(teamModel);
        }

        // GET: TeamModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamModel = await _context.Teams
                .FirstOrDefaultAsync(m => m.id == id);
            if (teamModel == null)
            {
                return NotFound();
            }

            return View(teamModel);
        }

        // POST: TeamModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teamModel = await _context.Teams.FindAsync(id);
            _context.Teams.Remove(teamModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamModelExists(int id)
        {
            return _context.Teams.Any(e => e.id == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //TODO
        public async Task<IActionResult> Search(string search)
        {
            List<TeamModel> teams = await _context.Teams.Include(t=>t.project).Include(t => t.devs).Include(t => t.teamLeader).ToListAsync();
            if (await extractUser())
            {
                if (search != null)
                {
                    string searchBy = Request.Form["SearchBy"];
                    if (searchBy == "projectName")
                    {
                        teams = teams.Where(t =>t.project!=null && t.project.name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                        ViewData["Message"] = "Searched by project name!";
                        return viewByTypeUser(teams);
                    }
                    else 
                    {
                         teams = teams.Where(t => t.name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

                        ViewData["Message"] = "Searched by team name!";
                        return viewByTypeUser(teams);
                    }              
                }
                else
                {
                    ViewData["Message"] = "You must type in searchbox!";
                    return viewByTypeUser(teams);
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        [NonAction]
        private ViewResult viewByTypeUser(object model)
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
