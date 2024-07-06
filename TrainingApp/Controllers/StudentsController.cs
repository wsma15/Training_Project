using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    public class StudentsController : Controller
    {
        TrainingAppDBContext MyDB = new TrainingAppDBContext();

        // GET: Students
        public ActionResult Index()
        {
            List<Student> studentsList = new List<Student>();
            studentsList = (from student in MyDB.Students select student).ToList();

            return View();
        }
        [HttpGet]
        public ActionResult AddReport()
        {
            var model = new Reports();
            model.ReportDateCreated = DateTime.Now;
            return View(model);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReport(Reports report)
        {
            try
            {
                {
                    report.ReportId = GenerateNewReportId();
                    report.OwnerId = User.Identity.GetUserId();
                    report.SupervisorID = (from superid in MyDB.Students
                                           where superid.StudentID == report.OwnerId
                                           select superid.SupervisorID).FirstOrDefault();
                    MyDB.Reports.Add(report);
                    MyDB.SaveChanges();
                    return RedirectToAction("StudentDashboard", "Students");
                }
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
                ModelState.AddModelError("", "An error occurred while saving the report.");
                return View(report);
            }
        }

        private int GenerateNewReportId()
        {
            int latestReportId = MyDB.Reports.Any()
                ? MyDB.Reports.OrderByDescending(m => m.ReportId).First().ReportId
                : 0;
            return ++latestReportId;
        }

        public ActionResult StudentDashboard()
        {
            var studentId = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(studentId))
            {
                return View((new List<Reports>()));
            }
            var reports = MyDB.Reports.Where(r => r.OwnerId == studentId).ToList();
            return View(reports);
        }

        public ActionResult GetStudents()
        {
            List<Student> StudentsList = new List<Student>();
            StudentsList = (from obj in MyDB.Students select obj).ToList();
            return View();
        }

        public ActionResult GetStudentDetails(string StudentId)
        {
            Student student = new Student();
            student = (from obj in MyDB.Students
                       where obj.StudentID == StudentId
                       select obj).FirstOrDefault();
            return View();
        }
        public ActionResult DeleteStudent(string StudentId)
        {
            Student student = new Student();
            student = (from obj in MyDB.Students
                       where obj.StudentID == StudentId
                       select obj).FirstOrDefault();
            MyDB.Students.Remove(student);
            MyDB.SaveChanges();

            return View("GetStudents");
        }
        public ActionResult UpdateStudent(string StudentId)
        {
            Student student = new Student();
            student = (from obj in MyDB.Students
                       where obj.StudentID == StudentId
                       select obj).FirstOrDefault();
            MyDB.Students.AddOrUpdate(student);
            MyDB.SaveChanges();

            return View("GetStudents");
        }
        public ActionResult InsertStudent(string super, string pass, string Name, string ID, string Email)
        {

            Student student = new Student()
            {
                SupervisorID = super,
                StudentPassword = pass,
                StudentName = Name,
                StudentID = ID,
                StudentEmail = Email
            };
            MyDB.Students.Add(student);
            MyDB.SaveChanges();
            return View("GetStudents");

        }

    }
}