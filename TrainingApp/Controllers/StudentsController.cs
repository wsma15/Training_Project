using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

        private int GetCurrentStudentId()
        {
            var studentIdString = User.Identity.Name; // Get student ID from authentication cookie (stored as string)

            // Convert to int or guid as needed
            if (int.TryParse(studentIdString, out int studentId))
            {
                return studentId;
            }

            return -1; // Handle if conversion fails
        }
        string CurrentId; public ActionResult StudentDashboard()
        {
            // Get current student's ID from claims
            var studentId = User.Identity.GetUserId();

            // Ensure studentId is not null or empty
            if (string.IsNullOrEmpty(studentId))
            {
                // Handle the case where studentId is not available (optional)
                // You might redirect to a login page or handle it based on your application logic
                // For simplicity, you can return an empty view or a message indicating an error

                return View((new List<Reports>())); // Return an empty list of reports
            }

            // Retrieve student's reports from database based on studentId
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