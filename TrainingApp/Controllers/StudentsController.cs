using Microsoft.AspNet.Identity;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    public class StudentsController : Controller
    {
        TrainingAppDBContext MyDB = new TrainingAppDBContext();

        [HttpGet]
        public ActionResult AddReport()
        {
            var model = new Reports
            {
                ReportDateCreated = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReport(Reports report, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                report.FileName = Path.GetFileName(file.FileName);
                report.ContentType = file.ContentType;
                using (var reader = new BinaryReader(file.InputStream))
                {
                    report.Content = reader.ReadBytes(file.ContentLength);
                }
            }
            else
            {
                ModelState.AddModelError("FileName", "Report File is required");
            }

            report.OwnerId = User.Identity.GetUserId();
            var student = MyDB.Users.SingleOrDefault(s => s.Id.ToString() == report.OwnerId && s.UniversitySupervisorID != null);
            if (student != null)
            {
                report.SupervisorID = student.UniversitySupervisorID;
            }
            else
            {
                ModelState.AddModelError("OwnerId", "Student not found");
            }

            //            if (ModelState.IsValid)
            {
                //   return Content(report.IsFeedbackSubmitted.ToString());
                MyDB.Reports.Add(report);
                MyDB.SaveChanges();
                return RedirectToAction("StudentDashboard", "Students");
            }
            //      else
            {
                return View(report);
            }
        }

        public ActionResult DeleteReport(int id)
        {
            var report = MyDB.Reports.Find(id);
            if (report != null)
            {
                MyDB.Reports.Remove(report);
                MyDB.SaveChanges();
            }
            return RedirectToAction("StudentDashboard");
        }

        public ActionResult StudentDashboard()
        {
            var studentId = User.Identity.GetUserId();
            var reports = MyDB.Reports.Where(r => r.OwnerId == studentId).ToList();
            return View(reports);
        }

        public ActionResult DownloadFile(int id)
        {
            var report = MyDB.Reports.Find(id);
            if (report != null && report.Content != null)
            {
                return File(report.Content, report.ContentType, report.FileName);
            }
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitFeedback(int Id, string feedback)
        {
            var report = MyDB.Reports.SingleOrDefault(r => r.Id == Id);
            if (report != null && !report.IsFeedbackSubmitted)
            {
                report.Feedback = feedback;
                report.IsFeedbackSubmitted = true;
                MyDB.SaveChanges();
            }

            return RedirectToAction("StudentDashboard", "Students");
        }
    }
}
