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
                Students = _context.Students.ToList(),
                Supervisors = _context.Supervisors.ToList(),


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
                // Generate a unique 10-digit ID
                int maxAttempts = 100; // Maximum number of attempts to generate a unique ID
                int attemptCount = 0;
                int generatedId;

                using (var context = new TrainingAppDBContext())
                {
                    bool idExists;
                    do
                    {
                        // Generate a random 10-digit ID
                        Random random = new Random();
                        generatedId = random.Next(10000, 99999);

                        // Check if the generated ID already exists in the database
                        idExists = context.Students.Any(s => s.StudentID == generatedId.ToString());

                        attemptCount++;

                    } while (idExists && attemptCount < maxAttempts);

                    if (attemptCount >= maxAttempts)
                    {
                        // Unable to generate a unique ID after maximum attempts
                        ModelState.AddModelError("", "Unable to generate a unique ID. Please try again later.");
                        return View(model);
                    }

                    // Save the student to the database using the generated ID
                    context.Students.Add(new Student
                    {

                        StudentID = generatedId.ToString(),
                        StudentName = model.StudentName,
                        StudentEmail = model.StudentEmail,
                        StudentPassword = model.StudentPassword,
                        SupervisorID = model.SupervisorID,

                        // Set other properties as needed
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
                    // Generate and assign a unique supervisor ID
                    var supervisorID = GenerateUniqueSupervisorID().ToString();

                    var supervisor = new Supervisor
                    {
                        SupervisorID = supervisorID,
                        SupervisorName = model.SupervisorName,
                        SupervisorEmail = model.SupervisorEmail,
                        SupervisorPassword = model.SupervisorPassword
                    };

                    // Log the supervisor details before saving
                    ModelState.AddModelError("", ($"Supervisor Details: ID={supervisor.SupervisorID}, Name={supervisor.SupervisorName}, Email={supervisor.SupervisorEmail}"));

                    _context.Supervisors.Add(supervisor);
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

        private int GenerateUniqueSupervisorID()
        {
            int maxAttempts = 100; // Maximum number of attempts to generate a unique ID
            int attemptCount = 0;
            int generatedId;
            bool idExists;

            do
            {
                // Generate a random ID
                Random random = new Random();
                generatedId = random.Next(100, 999);

                // Check if the generated ID already exists in the database
                idExists = _context.Supervisors.Any(s => s.SupervisorID == generatedId.ToString());

                attemptCount++;

            } while (idExists && attemptCount < maxAttempts);

            if (attemptCount >= maxAttempts)
            {
                // Unable to generate a unique ID after maximum attempts
                throw new Exception("Unable to generate a unique supervisor ID. Please try again later.");
            }

            return generatedId;
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
