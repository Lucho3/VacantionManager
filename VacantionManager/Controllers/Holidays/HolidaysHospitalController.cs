using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacantionManager.Models;
using VacantionManager.Models.Entity;

namespace VacantionManager.Controllers.Holidays
{
    public class HolidaysHospitalController : Controller
    {
        private readonly VacantionManagerDBContext _context;

        private readonly ILogger<HolidaysHospitalController> _logger;

        private UserModel user = null;

        public HolidaysHospitalController(VacantionManagerDBContext context, ILogger<HolidaysHospitalController> logger)
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
                List<HospitalLeaveModel> holidays = await _context.HospitalLeaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => l.approved == false).ToListAsync();
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
                List<HospitalLeaveModel> holidays = await _context.HospitalLeaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => l.approved == true).ToListAsync();
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
                List<HospitalLeaveModel> holidays = await _context.HospitalLeaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => l.approved == true && l.applicant == user).ToListAsync();
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
                List<HospitalLeaveModel> holidays = await _context.HospitalLeaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => l.approved == false && l.applicant == user).ToListAsync();
                return HolidayViewBasedOnUserRoleAsync(user.role.name, true, holidays);
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
                List<HospitalLeaveModel> holidays = await _context.HospitalLeaves.Include(l => l.applicant).ThenInclude(u => u.role).Where(l => DateTime.Compare(l.appicationDate, applicationDate) >= 0).ToListAsync();

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
                HospitalLeaveModel application = await _context.HospitalLeaves.FindAsync(id);
                user = await _context.Users.Include(u => u.leadedTeam).ThenInclude(u => u.devs).FirstOrDefaultAsync(u => u.id == user.id);
                if (user.role.name == "CEO" ||
                    (user.role.name == "Team Lead" &&
                    user.leadedTeam.devs.Any(d => d.hospitalLeaves.Contains(application))))
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
        private ViewResult HolidayViewBasedOnUserRoleAsync(string userRole, bool myRequests, List<HospitalLeaveModel> holidays)
        {
            if (userRole == "CEO" && myRequests == false)
            {
                ViewData["Approvable"] = "OK";
                return View("~/Views/Holidays/HolidaysHospitalCRUD/IndexCEOAndTEAMLead.cshtml", holidays.Where(l => l.applicant != user));
            }
            else if (user.role.name == "Team Lead" && myRequests == false)
            {
                ViewData["Approvable"] = "OK";
                return View("~/Views/Holidays/HolidaysHospitalCRUD/IndexCEOAndTEAMLead.cshtml", holidays.Where(l => l.applicant.team == user.leadedTeam && l.applicant != user));
            }
            else
            {
                if ((user.role.name == "Team Lead") && myRequests == true)
                {
                    ViewData["Approvable"] = "NO";
                    return View("~/Views/Holidays/HolidaysHospitalCRUD/IndexCEOAndTEAMLead.cshtml", holidays);
                }
                else if (userRole == "CEO")
                {
                    ViewData["Approvable"] = "OK";
                    return View("~/Views/Holidays/HolidaysHospitalCRUD/IndexCEOAndTEAMLead.cshtml", holidays);
                }
                else
                {
                    ViewData["Approvable"] = "NO";
                    return View("~/Views/Holidays/HolidaysHospitalCRUD/IndexNormalUser.cshtml", holidays.Where(u => u.applicant == user));
                }
            }
        }




        //////////Index end


        //Create

        // GET: HospitalLeaveModels/Create
        public async Task<IActionResult> Create()
        {
            if (await extractUser())
            {
                return View("~/Views/Holidays/HolidaysHospitalCRUD/Create.cshtml");
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        // POST: HospitalLeaveModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,startDate,endDate,appicationDate,approved")] HospitalLeaveModel HospitalLeaveModel, IFormFile abCard) 
        {
            if (await extractUser())
            {
                ModelState.Remove("applicant");
                ModelState.Remove("ambulatoryCard");
                if (ModelState.IsValid && HospitalLeaveModel.startDate >= DateTime.Now && HospitalLeaveModel.startDate <= HospitalLeaveModel.endDate && abCard!=null)
                {
                    using (var ms = new MemoryStream())
                    {
                        abCard.CopyTo(ms);
                        HospitalLeaveModel.ambulatoryCard = ms.ToArray();
                    }

                    HospitalLeaveModel.applicant = user;
                    _context.Add(HospitalLeaveModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("IndexDisapproved");
                }
                return View("~/Views/Holidays/HolidaysHospitalCRUD/Create.cshtml", HospitalLeaveModel);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        //Create end

        //Details
        // GET: HospitalLeaveModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (await extractUser())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var HospitalLeaveModel = await _context.HospitalLeaves.Include(l => l.applicant)
                    .FirstOrDefaultAsync(m => m.id == id);
                if (HospitalLeaveModel == null)
                {
                    return NotFound();
                }

                return View("~/Views/Holidays/HolidaysHospitalCRUD/Details.cshtml", HospitalLeaveModel);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }
        //Details end


        // GET: HospitalLeaveModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (await extractUser())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var HospitalLeaveModel = await _context.HospitalLeaves.FindAsync(id);

                if (HospitalLeaveModel == null)
                {
                    return NotFound();
                }

                if (HospitalLeaveModel.approved == false)
                {
                    return View("~/Views/Holidays/HolidaysHospitalCRUD/Edit.cshtml", HospitalLeaveModel);
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


        public async Task<IActionResult> DownloadAmbulatoryCard(int? id)
        {
            if (await extractUser())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var HospitalLeaveModel = await _context.HospitalLeaves.FindAsync(id);

                if (HospitalLeaveModel == null)
                {
                    return NotFound();
                }

                if (HospitalLeaveModel.ambulatoryCard == null)
                {
                    ViewData["Message"] = "No ambulatory card!";
                }
                else
                {

                    var fileToRetrieve = HospitalLeaveModel.ambulatoryCard;
                    return File(fileToRetrieve, "application/pdf", "Ambulatory_Card.pdf");
                }
                return View("~/Views/Holidays/HolidaysHospitalCRUD/Edit.cshtml", HospitalLeaveModel);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }
            // POST: HospitalLeaveModels/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,isPaid,startDate,endDate,appicationDate,halfDay")] HospitalLeaveModel HospitalLeaveModel, IFormFile abCard)
        {
            if (await extractUser())
            {
                if (id != HospitalLeaveModel.id)
                {
                    return NotFound();
                }

                ModelState.Remove("ambulatoryCard");
                ModelState.Remove("applicant");
                if (ModelState.IsValid)
                {
                    if (HospitalLeaveModel.approved == false)
                    {
                        try
                        {
                            HospitalLeaveModel UpdatedModel = await _context.HospitalLeaves.FindAsync(id);
                            UpdatedModel.startDate = HospitalLeaveModel.startDate;
                            UpdatedModel.applicant = user;
                            if (abCard!=null)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    abCard.CopyTo(ms);
                                    UpdatedModel.ambulatoryCard = ms.ToArray();
                                }
                            }
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!HospitalLeaveModelExists(HospitalLeaveModel.id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        //TODO: da vrushta dr action i viewdata
                        return RedirectToAction(nameof(IndexDisapproved));
                    }
                    else
                    {
                        return View("NoPermission");
                    }
                }
                return View("~/Views/Holidays/HolidaysHospitalCRUD/Edit.cshtml", HospitalLeaveModel);
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        // GET: HospitalLeaveModels/Delete/5
        //TODO: Validation 
        public async Task<IActionResult> Delete(int? id)
        {
            if (await extractUser())
            {
                if (id == null)
                {
                    return NotFound();
                }

                var HospitalLeaveModel = await _context.HospitalLeaves
                    .FirstOrDefaultAsync(m => m.id == id);

                if (HospitalLeaveModel == null)
                {
                    return NotFound();
                }

                return View("~/Views/Holidays/HolidaysHospitalCRUD/Delete.cshtml", HospitalLeaveModel);

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

        // POST: HospitalLeaveModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (await extractUser())
            {
                var HospitalLeaveModel = await _context.HospitalLeaves.FindAsync(id);
                _context.HospitalLeaves.Remove(HospitalLeaveModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexDisapproved));
            }
            else
            {
                return RedirectToAction("Index", "LogIn");
            }
        }

        private bool HospitalLeaveModelExists(int id)
        {
            return _context.HospitalLeaves.Any(e => e.id == id);
        }
    }
}
