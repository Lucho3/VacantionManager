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
            if (await extractUser())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var teamModel = await _context.Teams.Include(t => t.devs).ThenInclude(l => l.role).Include(t => t.project).Include(t => t.teamLeader)
                    .FirstOrDefaultAsync(m => m.id == id);

                if (teamModel == null)
                {
                    return NotFound();
                }

                if (teamModel.teamLeader != null)
                {
                    teamModel.devs.Remove(teamModel.teamLeader);
                }
                


                return await TeamDetailsView(teamModel);
                
            }

            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> RemoveFromTeam(int? userId, int? teamId)
        {
            if (await extractUser())
            {
                if (userId == null || teamId==null)
                {
                    return NotFound();
                }

                UserModel u = await _context.Users.Where(u => u.id == userId).Include(u => u.team).FirstOrDefaultAsync();
                var teamModel = await _context.Teams.Include(t => t.devs).Include(t => t.project).Include(t => t.teamLeader)
                    .FirstOrDefaultAsync(m => m.id == teamId);

                if (teamModel == null)
                {
                    return NotFound();
                }
                
                teamModel.devs.Remove(u);
                u.team = null;
                await _context.SaveChangesAsync();

                //TODO: Refaactor
                return await TeamDetailsView(teamModel);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> AddInTeam(int? userId, int? teamId)
        {
            if (await extractUser())
            {
                if (userId == null || teamId==null)
                {
                    return NotFound();
                }

                UserModel u = await _context.Users.Include(u => u.team).Include(u=>u.role).FirstOrDefaultAsync(u => u.id == userId);
                var teamModel = await _context.Teams.Include(t => t.devs).Include(t => t.project).Include(t => t.teamLeader)
                    .FirstOrDefaultAsync(m => m.id == teamId);

                if (teamModel == null)
                {
                    return NotFound();
                }

                if (teamModel.teamLeader != null && u.role.name == "Team Lead")
                {
                    ViewData["Message"] = "This user can't be add. The team already has team leader!";
                    return await TeamDetailsView(teamModel);
                }
                //TODO:: REFACTOR
                else
                {
                    if (u.role.name == "Team Lead")
                    {
                        u.leadedTeam = teamModel;
                        u.team = teamModel;
                        teamModel.devs.Add(u);
                        u.team = teamModel;
                    }
                    else
                    {
                        teamModel.devs.Add(u);
                        u.team = teamModel;
                    }
                    await _context.SaveChangesAsync();
                    if (teamModel.teamLeader != null)
                    {
                        teamModel.devs.Remove(teamModel.teamLeader);
                    }
                    return await TeamDetailsView(teamModel);
                }
                
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
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
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    TeamModel teamModel = await _context.Teams.Include(t => t.teamLeader).Include(t => t.project).FirstOrDefaultAsync(t => t.id == id);
                    if (teamModel == null)
                    {
                        return NotFound();
                    }

                    Dictionary<string, string> users = await _context.Users.Include(u => u.role).Where(u => u.role.name == "Team Lead" && u.team == null).ToDictionaryAsync(u => u.username, u => (u.username+"("+u.firstName + " " + u.lastName+")"));
                    List<string> projects = await _context.Projects.Select(p => p.name).ToListAsync();
                    TeamEditViewModel tevm = new TeamEditViewModel(teamModel, users, projects);
                    return View(tevm);
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

        //TODO refactor these 3 methods

        public async Task<IActionResult> ChangeTeamName(int? teamId,string teamName)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (teamId== null)
                    {
                        return NotFound();
                    }

                    TeamModel team = await _context.Teams.Include(t => t.teamLeader).Include(t => t.project).FirstOrDefaultAsync(t => t.id == teamId);
                    Dictionary<string, string> users = await _context.Users.Include(u => u.role).Where(u => u.role.name == "Team Lead" && u.team == null).ToDictionaryAsync(u => u.username, u => (u.username + "(" + u.firstName + " " + u.lastName + ")"));
                    List<string> projects = await _context.Projects.Select(p => p.name).ToListAsync();
                    
                    TeamEditViewModel tevm = new TeamEditViewModel(team, users, projects);

                    if (teamName != null)
                    {
                        team.name = teamName;
                        await _context.SaveChangesAsync();

                        return View("Edit", tevm);
                    }
                    else
                    {
                        ViewData["Message"] = "You must choose name for the team!";
                        return View("Edit", tevm);
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

        public async Task<IActionResult> ChangeLeader(int? teamId)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (teamId == null)
                    {
                        return NotFound();
                    }

                    string leader = Request.Form["Leaders"];

                    TeamModel team = await _context.Teams.Include(t => t.teamLeader).Include(t => t.project).FirstOrDefaultAsync(t => t.id == teamId);
                    Dictionary<string, string> users = await _context.Users.Include(u => u.role).Where(u => u.role.name == "Team Lead" && u.team == null).ToDictionaryAsync(u => u.username, u => (u.username + "(" + u.firstName + " " + u.lastName + ")"));
                    List<string> projects = await _context.Projects.Select(p => p.name).ToListAsync();
                    if (team.teamLeader!=null)
                    {
                        UserModel oldLeader = team.teamLeader;
                        oldLeader.team = null;
                        oldLeader.leadedTeam = null;
                    }
                    

                    UserModel newLeader = await _context.Users.Include(u => u.team).Include(u => u.leadedTeam).FirstOrDefaultAsync(u => u.username == leader);
                    if (newLeader != null)
                    {
                        newLeader.team = team;
                        newLeader.leadedTeam = team;
                        TeamEditViewModel tevm = new TeamEditViewModel(team, users, projects);
                        await _context.SaveChangesAsync();
                        return View("Edit", tevm);
                    }
                    else
                    {
                        TeamEditViewModel tevm = new TeamEditViewModel(team, users, projects);
                        await _context.SaveChangesAsync();
                        return View("Edit", tevm);
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

        public async Task<IActionResult> ChangeProject(int? teamId)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    if (teamId == null)
                    {
                        return NotFound();
                    }

                    string projectName = Request.Form["Projects"];

                    TeamModel team = await _context.Teams.Include(t => t.teamLeader).Include(t => t.project).FirstOrDefaultAsync(t => t.id == teamId);
                    Dictionary<string, string> users = await _context.Users.Include(u => u.role).Where(u => u.role.name == "Team Lead" && u.team == null).ToDictionaryAsync(u => u.username, u => (u.username + "(" + u.firstName + " " + u.lastName + ")"));
                    List<string> projects = await _context.Projects.Select(p => p.name).ToListAsync();

                    ProjectModel newProject = await _context.Projects.FirstOrDefaultAsync(p => p.name == projectName);
                    if (newProject != null)
                    {
                        team.project = newProject;
                        TeamEditViewModel tevm = new TeamEditViewModel(team, users, projects);
                        await _context.SaveChangesAsync();
                        return View("Edit", tevm);
                    }
                    else
                    {
                        team.project = null;
                        TeamEditViewModel tevm = new TeamEditViewModel(team, users, projects);
                        await _context.SaveChangesAsync();
                        return View("Edit", tevm);
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



        // GET: TeamModels/Delete/5
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

                    var teamModel = await _context.Teams
                        .FirstOrDefaultAsync(m => m.id == id);
                    if (teamModel == null)
                    {
                        return NotFound();
                    }

                    return View(teamModel);

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

        // POST: TeamModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await extractUser())
            {
                if (user.role.name == "CEO")
                {
                    var teamModel = await _context.Teams.FindAsync(id);
                    _context.Teams.Remove(teamModel);
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
            
            if (await extractUser())
            {
                List<TeamModel> teams = await _context.Teams.Include(t => t.project).Include(t => t.devs).Include(t => t.teamLeader).ToListAsync();
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
                user = await _context.Users.Where(u => u.id == userId).Include(u => u.role).Include(u=>u.leadedTeam).Include(u=>u.team).FirstOrDefaultAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        [NonAction]
        private async Task<ViewResult> TeamDetailsView(TeamModel teamModel)
        {
            if (user.role.name == "CEO")
            {
                List<UserModel> users = await _context.Users.Include(u => u.team).Include(u=>u.role).Where(u => u.team == null).ToListAsync();
                TeamDetailsViewModel tdvm = new TeamDetailsViewModel(teamModel, users);
                return View("Details", tdvm);
            }
            else if (user.role.name == "Team Lead" && user.leadedTeam == teamModel)
            {
                List<UserModel> users = await _context.Users.Include(u => u.team).Include(u => u.role).Where(u => u.team == null).ToListAsync();
                TeamDetailsViewModel tdvm = new TeamDetailsViewModel(teamModel, users);
                return View("DetailsTeamLead", tdvm);
            }
            else
            {
                return View("DetailsNormalUser", teamModel);
            }
        }
    }
}
