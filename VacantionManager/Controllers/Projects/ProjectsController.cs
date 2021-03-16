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

namespace VacantionManager.Controllers.Projects
{
    public class ProjectsController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<ProjectsController> _logger;

        private UserModel user = null;

        public ProjectsController(VacantionManagerDBContext context, ILogger<ProjectsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            if (await extractUser())
            {
                List<ProjectModel> projects = await _context.Projects.Include(p=>p.workingTeams).ToListAsync();
                if (user.role.name == "CEO")
                {
                    return View(projects);
                }
                else
                {
                    return View("IndexNormalUser", projects);
                }
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (await extractUser())
            {

                    if (id == null)
                    {
                        return NotFound();
                    }

                    ProjectModel projectModel = await _context.Projects.Include(p=>p.workingTeams).ThenInclude(t=>t.teamLeader)
                        .FirstOrDefaultAsync(m => m.id == id);

                    if (projectModel == null)
                    {
                        return NotFound();
                    }

                return await ProjectDetailsView(projectModel);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> RemoveFromProject(int? projectId, int? teamId)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (projectId == null || teamId == null)
                {
                    return NotFound();
                }

                TeamModel teamForRemoval = await _context.Teams.FirstAsync(t => t.id == teamId);
                ProjectModel projectModel = await _context.Projects.Include(p => p.workingTeams).ThenInclude(t => t.teamLeader)
                       .FirstOrDefaultAsync(m => m.id == projectId);

                if (projectModel == null)
                {
                    return NotFound();
                }

                projectModel.workingTeams.Remove(teamForRemoval);
                teamForRemoval.project = null;
                await _context.SaveChangesAsync();

                    //TODO: Refaactor
                    return await ProjectDetailsView(projectModel);
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

        public async Task<IActionResult> AddTeamToProject(int? projectId, int? teamId)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (projectId == null || teamId == null)
                    {
                        return NotFound();
                    }

                    TeamModel teamToAdd = await _context.Teams.Include(t=>t.project).FirstAsync(t => t.id == teamId);
                    ProjectModel projectModel = await _context.Projects.Include(p => p.workingTeams).ThenInclude(t => t.teamLeader)
                           .FirstOrDefaultAsync(m => m.id == projectId);

                    if (projectModel == null)
                    {
                        return NotFound();
                    }

                    projectModel.workingTeams.Add(teamToAdd);
                    teamToAdd.project = projectModel;
                    await _context.SaveChangesAsync();

                    //TODO: Refaactor
                    return await ProjectDetailsView(projectModel);
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

        // GET: Projects/Create
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

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,description")] ProjectModel projectModel)
        {
            if (await extractUser())
            {
                if (ModelState.IsValid)
                {
                    if (user.role.name == "CEO")
                    {
                        _context.Add(projectModel);
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
                    return View(projectModel);
                }

            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        // GET: Projects/Edit/5
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

                    var projectModel = await _context.Projects.FindAsync(id);
                    if (projectModel == null)
                    {
                        return NotFound();
                    }
                    return View(projectModel);
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

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,description")] ProjectModel projectModel)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {

                    if (id != projectModel.id)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(projectModel);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ProjectModelExists(projectModel.id))
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
                    return View(projectModel);
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

        // GET: Projects/Delete/5
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

                    var projectModel = await _context.Projects
                        .FirstOrDefaultAsync(m => m.id == id);
                    if (projectModel == null)
                    {
                        return NotFound();
                    }

                    return View(projectModel);
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

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    var projectModel = await _context.Projects.FindAsync(id);
                    _context.Projects.Remove(projectModel);
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

        private bool ProjectModelExists(int id)
        {
            return _context.Projects.Any(e => e.id == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Search(string search)
        {
            if (await extractUser())
            {
                List<ProjectModel> projects = await _context.Projects.ToListAsync();
                if (search != null)
                {
                    string searchBy = Request.Form["SearchBy"];
                    if (searchBy == "projectName")
                    {
                        projects = projects.Where(p => p.name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                        ViewData["Message"] = "Searched by project name!";
                        return viewByTypeUser(projects);
                    }
                    else
                    {
                        projects = projects.Where(p => p.description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                        ViewData["Message"] = "Searched by project description!";
                        return viewByTypeUser(projects);
                    }
                }
                else
                {
                    ViewData["Message"] = "You must type in searchbox!";
                    return viewByTypeUser(projects);
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
                return View("INdexNormalUser", model);
            }
        }

        [NonAction]
        private async Task<bool> extractUser()
        {
            byte[] buffer = new byte[200];
            if (HttpContext.Session.TryGetValue("id", out buffer))
            {
                int userId = int.Parse(Encoding.UTF8.GetString(buffer));
                user = await _context.Users.Where(u => u.id == userId).Include(u => u.role).Include(u => u.leadedTeam).Include(u => u.team).FirstOrDefaultAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        [NonAction]
        private async Task<ViewResult> ProjectDetailsView(ProjectModel projectModel)
        {
            if (user.role.name == "CEO")
            {
                List<TeamModel> freeTeams = await _context.Teams.Include(t => t.teamLeader).Include(t => t.project).Where(t => t.project == null).ToListAsync();
                ProjectDetailsViewModel project = new ProjectDetailsViewModel(projectModel, freeTeams);
                return View("Details",project);
            }
            else
            {
                return View("DetailsNormalUser",projectModel); ;
            }

        }
    }
}
