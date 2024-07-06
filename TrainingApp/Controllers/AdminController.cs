using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;
using TrainingApp.ViewModels;

namespace TrainingApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();
        // GET: /Admin/AdminDashboard
        //  [Authorize(Roles = "Admin")]
        public ActionResult AdminDashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                Students = _context.Students.ToList(),
                Supervisors = _context.Supervisors.ToList()
            };

            return View(viewModel);
        }


        public ActionResult AddStudent()
        {
            var viewModel = new AddStudentViewModel
            {
                Supervisors = _context.Supervisors.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStudent(AddStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = new Student
                {
                    StudentName = model.StudentName,
                    StudentEmail = model.StudentEmail,
                    StudentPassword = model.StudentPassword,
                    SupervisorID = model.SupervisorID
                };

                _context.Students.Add(student);
                _context.SaveChanges();

                return RedirectToAction("AdminDashboard");
            }

            model.Supervisors = _context.Supervisors.ToList();
            return View(model);
        }

        public ActionResult AddSupervisor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSupervisor(AddSupervisorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var supervisor = new Supervisor
                {
                    SupervisorName = model.SupervisorName,
                    SupervisorEmail = model.SupervisorEmail,
                    SupervisorPassword = model.SupervisorPassword
                };

                _context.Supervisors.Add(supervisor);
                _context.SaveChanges();

                return RedirectToAction("AdminDashboard");
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
