using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    public class SupervisorController : Controller
    {
        private TrainingAppDBContext db = new TrainingAppDBContext();

        public ActionResult DownloadFile(int reportId)
        {
            var report = db.Reports.Find(reportId);
            if (report != null && !string.IsNullOrEmpty(report.FileName))
            {
                // Return the file using FileResult
                return File(report.Content, System.Net.Mime.MediaTypeNames.Application.Octet, report.FileName);
            }
            return HttpNotFound();
        }

        // GET: Supervisor/Dashboard
        public ActionResult Dashboard()
        {
            string supervisorId = User.Identity.GetUserId(); // Assuming UserId is the SupervisorID
            var students = db.Students.Where(s => s.SupervisorID == supervisorId).ToList();
            return View(students);
        }
        // GET: Supervisor/StudentReports/5
        public ActionResult StudentReports(string id)
        {
            var reports = db.Reports.Where(r => r.OwnerId == id).ToList();
            return View(reports);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitFeedback(int reportId, bool isApproved, string feedback)
        {
            var report = db.Reports.SingleOrDefault(r => r.ReportId == reportId);
            if (report != null && !report.IsFeedbackSubmitted)
            {
                report.IsApproved = isApproved;
                report.Feedback = feedback;
                report.IsFeedbackSubmitted = true;
                db.SaveChanges();
            }

            return RedirectToAction("Dashboard", "Supervisor");
        }
        public ActionResult Reports()
        {
            var reports = db.Reports.ToList();
            return View(reports);
        }
    }
}
