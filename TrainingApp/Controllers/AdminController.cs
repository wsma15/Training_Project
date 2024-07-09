using System;
using System.Data.Entity.Infrastructure;
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
                Students = _context.Users
                                  .Where(super => super.Roles == UserRole.Student)
                                  .ToList(),
                Supervisors = _context.Users
                                  .Where(super => super.Roles == UserRole.Supervisor)
                                  .ToList()

,


            };

            return View(viewModel);
        }


        public ActionResult AddStudent()
        {
            var viewModel = new AddStudentViewModel
            {
                Supervisors = _context.Users
                                  .Where(super => super.Roles == UserRole.Supervisor)
                                  .ToList(),
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStudent(AddStudentViewModel model)
        {
            if (ModelState.IsValid)
            {

                using (var context = new TrainingAppDBContext())
                {
                    // Save the student to the database using the generated ID
                    context.Users.Add(new Users
                    {

                        Name = model.StudentName,
                        Email = model.StudentEmail,
                        Password = model.StudentPassword,
                        SupervisorID = model.SupervisorID.ToString(),
                        Roles = UserRole.Student
                    });
                    context.SaveChanges();
                }

                // Redirect to the list of students or another appropriate page
                return RedirectToAction("AdminDashboard", "Admin");
            }

            // If the model state is not valid, return the view with validation errors
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
                try
                {

                    var supervisor = new Users
                    {
                        Name = model.SupervisorName,
                        Email = model.SupervisorEmail,
                        Password = model.SupervisorPassword,
                        Roles = UserRole.Supervisor
                    };

                    // Log the supervisor details before saving
                    ModelState.AddModelError("", ($"Supervisor Details: ID={supervisor.SupervisorID}, Name={supervisor.Name}, Email={supervisor.Email}"));

                    _context.Users.Add(supervisor);
                    _context.SaveChanges();

                    return RedirectToAction("AdminDashboard", "Admin");
                }
                catch (DbUpdateException ex)
                {
                    // Log inner exceptions for more detail
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("Inner Exception Message: " + ex.InnerException.Message);
                        if (ex.InnerException.InnerException != null)
                        {
                            Console.WriteLine("Inner Inner Exception Message: " + ex.InnerException.InnerException.Message);
                        }
                    }
                    ModelState.AddModelError("", "An error occurred while adding the supervisor. Please try again.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception Message: " + ex.Message);
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
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
