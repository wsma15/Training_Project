using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity.Validation;
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
        public ActionResult SubmitFeedback(int reportId, string feedback)
        {
            var report = MyDB.Reports.SingleOrDefault(r => r.ReportId == reportId);
            if (report != null && !report.IsFeedbackSubmitted)
            {
                report.Feedback = feedback;
                report.IsFeedbackSubmitted = true;
                MyDB.SaveChanges();
            }

            // Redirect back to the student dashboard after submitting feedback
            return RedirectToAction("StudentDashboard", "Students");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReport(Reports report, HttpPostedFileBase file)
        {
            try
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

                report.ReportId = GenerateNewReportId();
                report.OwnerId = User.Identity.GetUserId();

                var student = MyDB.Students.SingleOrDefault(s => s.StudentID == report.OwnerId);
                if (student != null)
                {
                    report.SupervisorID = student.SupervisorID;
                }

                MyDB.Reports.Add(report);
                MyDB.SaveChanges();

                return RedirectToAction("StudentDashboard", "Students");
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return View(report);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the report: " + ex.Message);
                return View(report);
            }
        }

        private int GenerateNewReportId()
        {
            return MyDB.Reports.Any()
                ? MyDB.Reports.Max(r => r.ReportId) + 1
                : 1;
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

    }
}
