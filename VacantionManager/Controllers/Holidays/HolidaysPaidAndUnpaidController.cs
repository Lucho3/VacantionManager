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
    public class HolidaysPaidAndUnpaidController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<HolidaysPaidAndUnpaidController> _logger;

        private UserModel user = null;

        public HolidaysPaidAndUnpaidController(VacantionManagerDBContext context, ILogger<HolidaysPaidAndUnpaidController> logger)
        {
            _context = context;
            _logger = logger;
        }


        //Index
        public async Task<IActionResult> IndexDisapproved()
        {
            if (await extractUser())
            {
                ViewData["Requests"] = "Requests for approval:";
                List<LeaveModel> holidays = await _context.Leaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => l.approved == false).ToListAsync();
                return HolidayViewBasedOnUserRoleAsync(user.role.name, false, holidays);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }
        public async Task<IActionResult> IndexApproved()
        {
            if (await extractUser())
            {
                ViewData["Requests"] = "Approved requests:";
                List<LeaveModel> holidays = await _context.Leaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => l.approved == true).ToListAsync();
                return HolidayViewBasedOnUserRoleAsync(user.role.name, false, holidays);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> IndexMyApproved()
        {
            if (await extractUser())
            {
                ViewData["Requests"] = "My approved requests:";
                List<LeaveModel> holidays = await _context.Leaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => l.approved == true && l.applicant == user).ToListAsync();
                return HolidayViewBasedOnUserRoleAsync(user.role.name, true, holidays);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> IndexMyDisapproved()
        {
            if (await extractUser())
            {
                ViewData["Requests"] = "My disapproved requests:";
                List<LeaveModel> holidays = await _context.Leaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => l.approved == false && l.applicant == user).ToListAsync();
                return HolidayViewBasedOnUserRoleAsync(user.role.name, true, holidays);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> Filter()
        {
            if (await extractUser())
            {
                bool isPaidForm = false;
                if (Request.Form["FilterBy"] == "paid")
                {
                    isPaidForm = true;
                }

                ViewData["Requests"] = "Filtered by: " + Request.Form["FilterBy"] + "requests";
                List<LeaveModel> holidays = await _context.Leaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => l.isPaid == isPaidForm).ToListAsync();

                if (user.role.name == "CEO" || user.role.name == "Team Lead")
                {
                    return HolidayViewBasedOnUserRoleAsync(user.role.name, false, holidays);
                }
                else
                {
                    return HolidayViewBasedOnUserRoleAsync(user.role.name, true, holidays);
                }

            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> Search(DateTime applicationDate)
        {
            if (await extractUser())
            {
                ViewData["Requests"] = string.Format("Requests after {0}.{1}.{2}", applicationDate.Day, applicationDate.Month, applicationDate.Year);
                List<LeaveModel> holidays = await _context.Leaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => DateTime.Compare(l.appicationDate, applicationDate) >= 0).ToListAsync();

                if (user.role.name == "CEO" || user.role.name == "Team Lead")
                {

                    return HolidayViewBasedOnUserRoleAsync(user.role.name, false, holidays);
                }
                else
                {
                    return HolidayViewBasedOnUserRoleAsync(user.role.name, true, holidays);
                }

            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        public async Task<IActionResult> Approve(int? id)
        {
            if (await extractUser())
            {
                LeaveModel application = await _context.Leaves.FindAsync(id);
                user = await _context.Users.Include(u => u.leadedTeam).ThenInclude(u => u.devs).FirstOrDefaultAsync(u => u.id == user.id);
                if (user.role.name == "CEO" ||
                    (user.role.name == "Team Lead" &&
                    user.leadedTeam.devs.Any(d => d.leaves.Contains(application))))
                {
                    application.approved = true;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(IndexDisapproved));
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



        [NonAction]
        private ViewResult HolidayViewBasedOnUserRoleAsync(string userRole, bool myRequests, List<LeaveModel> holidays)
        {
            if (userRole == "CEO" && myRequests == false)
            {
                ViewData["Approvable"] = "OK";
                return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/IndexCEOAndTEAMLead.cshtml", holidays.Where(l => l.applicant != user));
            }
            else if (user.role.name == "Team Lead" && myRequests == false)
            {
                ViewData["Approvable"] = "OK";
                return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/IndexCEOAndTEAMLead.cshtml", holidays.Where(l => l.applicant.team == user.leadedTeam && l.applicant != user));
            }
            else
            {
                if ((user.role.name == "Team Lead") && myRequests == true)
                {
                    ViewData["Approvable"] = "NO";
                    return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/IndexCEOAndTEAMLead.cshtml", holidays);
                }
                else if (userRole == "CEO")
                {
                    ViewData["Approvable"] = "OK";
                    return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/IndexCEOAndTEAMLead.cshtml", holidays);
                }
                else
                {
                    ViewData["Approvable"] = "NO";
                    return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/IndexNormalUser.cshtml", holidays.Where(u => u.applicant == user));
                }
            }
        }




        //////////Index end


        //Create

        // GET: LeaveModels/Create
        public async Task<IActionResult> Create()
        {
            if (await extractUser())
            {
                return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/Create.cshtml");
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        // POST: LeaveModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,isPaid,startDate,endDate,appicationDate,halfDay,approved")] LeaveModel leaveModel)
        {
            if (await extractUser())
            {
                ModelState.Remove("applicant");
                if (ModelState.IsValid && leaveModel.startDate >= DateTime.Now && leaveModel.startDate <= leaveModel.endDate)
                {
                    leaveModel.applicant = user;
                    _context.Add(leaveModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("IndexDisapproved");
                }
                return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/Create.cshtml", leaveModel);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        //Create end

        //Details
        // GET: LeaveModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (await extractUser())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var leaveModel = await _context.Leaves.Include(l => l.applicant)
                    .FirstOrDefaultAsync(m => m.id == id);
                if (leaveModel == null)
                {
                    return NotFound();
                }

                return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/Details.cshtml", leaveModel);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }
        //Details end


        // GET: LeaveModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (await extractUser())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var leaveModel = await _context.Leaves.FindAsync(id);

                if (leaveModel == null)
                {
                    return NotFound();
                }

                if (leaveModel.approved == false)
                {
                    return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/Edit.cshtml", leaveModel);
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

        // POST: LeaveModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,isPaid,startDate,endDate,appicationDate,halfDay")] LeaveModel leaveModel)
        {
            if (await extractUser())
            {
                if (id != leaveModel.id)
                {
                    return NotFound();
                }

                ModelState.Remove("applicant");
                if (ModelState.IsValid)
                {
                    if (leaveModel.approved == false)
                    {
                        try
                        {
                            LeaveModel UpdatedModel = await _context.Leaves.FirstOrDefaultAsync(l => l.id == id);
                            UpdatedModel.startDate = leaveModel.startDate;
                            UpdatedModel.endDate = leaveModel.endDate;
                            UpdatedModel.isPaid = leaveModel.isPaid;
                            UpdatedModel.halfDay = leaveModel.halfDay;
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!LeaveModelExists(leaveModel.id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(IndexDisapproved));
                    }
                    else
                    {
                        return View("NoPermission");
                    }
                }
                return View(leaveModel);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        // GET: LeaveModels/Delete/5
        //TODO: Validation 
        public async Task<IActionResult> Delete(int? id)
        {
            if (await extractUser())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var leaveModel = await _context.Leaves
                    .FirstOrDefaultAsync(m => m.id == id);

                if (leaveModel == null)
                {
                    return NotFound();
                }

                return View("~/Views/Holidays/HolidaysPaidUnpaidCRUD/Delete.cshtml", leaveModel);

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
                user = await _context.Users.Where(u => u.id == userId).Include(u => u.role).Include(u => u.leadedTeam).FirstOrDefaultAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        // POST: LeaveModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await extractUser())
            {
                var leaveModel = await _context.Leaves.FindAsync(id);
                _context.Leaves.Remove(leaveModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexDisapproved));
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        private bool LeaveModelExists(int id)
        {
            return _context.Leaves.Any(e => e.id == id);
        }
    }
}
