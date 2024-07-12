using Microsoft.AspNet.Identity;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    public class TrainersController : Controller
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
            if (report.ReportDescription == null)
                report.ReportDescription = " ";
            if (file == null || report.ReportTitle == null)
            {
                //      ModelState.AddModelError("FileName", "Report File is required");

                return View(report);
            }

            report.OwnerId = User.Identity.GetUserId();
            var student = MyDB.Users.SingleOrDefault(s => s.Id.ToString() == report.OwnerId && s.UniversitySupervisorID != null);
            if (student != null)
            {
                report.SupervisorID = (int)student.UniversitySupervisorID;
            }
            else
            {
                ModelState.AddModelError("OwnerId", "Student not found");
            }

            {
                //   return Content(report.IsFeedbackSubmitted.ToString());
                MyDB.Reports.Add(report);
                MyDB.SaveChanges();
                return RedirectToAction("Dashboard", "Trainers");
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
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
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

            return RedirectToAction("Dashboard", "Trainers");
        }
    }
}
