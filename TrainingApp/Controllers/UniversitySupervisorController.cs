using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    public class UniversitySupervisorController : Controller
    {
        private TrainingAppDBContext db = new TrainingAppDBContext();

        public ActionResult DownloadFile(int Id)
        {
            var report = db.Reports.Find(Id);
            if (report != null && !string.IsNullOrEmpty(report.FileName))
            {
                // Return the file using FileResult
                return File(report.Content, System.Net.Mime.MediaTypeNames.Application.Octet, report.FileName);
            }
            return HttpNotFound();
        }
        [Authorize(Roles = "UniversitySupervisor")]

        // GET: Supervisor/Dashboard
        public ActionResult Dashboard()
        {
            int supervisorId = (int)Convert.ToInt64(User.Identity.GetUserId()); // Assuming UserId is the SupervisorID
            var Trainers = db.Users.Where(s => s.UniversitySupervisorID == supervisorId).ToList();
            return View(Trainers);
        }
        [Authorize(Roles = "UniversitySupervisor")]

        // GET: Supervisor/StudentReports/5
        public ActionResult StudentReports(string supervisorId)
        {
            var reports = db.Reports.Where(r=>r.OwnerId== supervisorId).ToList();
            return View(reports);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitFeedback(int Id, bool isApproved, string feedback)
        {
            var report = db.Reports.SingleOrDefault(r => r.Id == Id);
            if (report != null && !report.IsFeedbackSubmitted)
            {
                report.IsApproved = isApproved;
                report.Feedback = feedback;
                report.IsFeedbackSubmitted = true;
                db.SaveChanges();
            }

            return RedirectToAction("Dashboard", "UniversitySupervisor");
        }
        public ActionResult Reports()
        {
            var reports = db.Reports.ToList();
            return View(reports);
        }
    }
}
