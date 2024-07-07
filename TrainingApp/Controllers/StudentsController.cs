using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity.Validation;
using System.Linq;
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
        public ActionResult AddReport(Reports report)
        {
            try
            {
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

        public ActionResult StudentDashboard()
        {
            var studentId = User.Identity.GetUserId();
            var reports = MyDB.Reports.Where(r => r.OwnerId == studentId).ToList();
            return View(reports);
        }
    }
}
