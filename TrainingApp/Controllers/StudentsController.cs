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