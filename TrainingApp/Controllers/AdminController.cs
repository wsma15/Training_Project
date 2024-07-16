using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrainingApp.Models;
using TrainingApp.ViewModels;
namespace TrainingApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();
        // GET: /Admin/Dashboard
          [Authorize(Roles = "Admin")]
        public ActionResult Dashboard()
        {
        var viewModel = new DashboardViewModel
            {
                Trainers = _context.Users
                                  .Where(super => super.Roles == UserRole.Trainer)
                                  .ToList(),
                UniversitySupervisors = _context.Users
                                  .Where(super => super.Roles == UserRole.UniversitySupervisor)
                                  .ToList(),
                CompanySupervisors = _context.Users.Where(
                    super => super.Roles == UserRole.CompanySupervisor
                    ).ToList(),
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]

        public ActionResult AddStudent()
        {
            var viewModel = new AddTrainerViewModel
            {
                UniversitySupervisors = _context.Users
                                      .Where(u => u.Roles == UserRole.UniversitySupervisor)
                                      .Select(u => new SelectListItem
                                      {
                                          Value = u.Id.ToString(),
                                          Text = u.Name
                                      })
                                      .ToList(),

                // Retrieve list of company supervisors along with their company names
                CompanySupervisors = _context.Users
                                    .Where(u => u.Roles == UserRole.CompanySupervisor)
                                    .Select(u => new SelectListItem
                                    {
                                        Value = u.Id.ToString(),
                                        Text = u.CompanyName + " - " + u.Name // Display company name and supervisor name
                                    })
                                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddStudent(AddTrainerViewModel model)
        {
            if (ModelState.IsValid)
            {

                using (var context = new TrainingAppDBContext())
                {
                    // Retrieve the company name based on the selected supervisor ID
                    var companyName = context.Users
                                             .Where(u => u.Id == model.CompanySupervisorID)
                                             .Select(u => u.CompanyName)
                                             .FirstOrDefault();
                    var UniName = context.Users
                                 .Where(u => u.Id == model.UniversitySupervisorID)
                                 .Select(u => u.UniversityName)
                                 .FirstOrDefault();
                    var user = new Users
                    {
                        Name = model.TrainerName,
                        Email = model.TrainerEmail,
                        Password = model.TrainerPassword,
                        UniversitySupervisorID = model.UniversitySupervisorID,
                        CompanySupervisorID = model.CompanySupervisorID,
                        CompanyName = companyName, // Save the company name
                        Roles = UserRole.Trainer,
                        UniversityName = UniName,
                    };

                    context.Users.Add(user);
                    context.SaveChanges();
                    MailHelper.SendEmail(
        user.Email,
        "Welcome to the Training Management System",
        $"Dear {user.Name},\n\n" +
        "Welcome to the Training Management System (TMS)! We are delighted to have you join us.\n\n" +
        "Here are your account details:\n" +
        $"- **User ID:** {user.Id}\n" +
        $"- **Password:** {user.Password}\n\n" +
        "If you have any questions or need assistance, please do not hesitate to contact our support team.\n\n" +
        "Best regards,\n" +
        "The TMS Team"
    );
                }

                // Redirect to the admin dashboard after adding the student
                return RedirectToAction("Dashboard", "Admin");
            }

            // If the model state is not valid, return the view with validation errors
            return View(model);
        }
        public string GetUniName(int id)
        {
            return (from name in _context.Users where name.Id == id select name.UniversityName).FirstOrDefault();


        }
        public ActionResult AddUniversitySupervisor()
        {
            return View();
        }
        public ActionResult AddSupervisor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public ActionResult AddUniversitySupervisor(AddSupervisorViewModel model)
        {
           // if (ModelState.IsValid)
            {
                // Perform actions to add supervisor to database
                using (var context = new TrainingAppDBContext())
                {
                    // Create a new user instance
                    var newUser = new Users
                    {
                        Name = model.SupervisorName,
                        Email = model.SupervisorEmail,
                        Password = model.SupervisorPassword, // Ensure you are hashing passwords in a real application
                        UniversityName = model.UniversityName, // Assuming you have this property in your Users model
                        Roles = UserRole.UniversitySupervisor // Adjust roles as per your application logic
                    };

                    // Add the user to the database and save changes
                    context.Users.Add(newUser);
                    context.SaveChanges();

                    // Retrieve the ID of the newly added user
                    int newUserId = newUser.Id;

                    // Send the welcome email with the user ID and password
                   MailHelper.SendEmail(
                        model.SupervisorEmail,
                        "Welcome to the Training Management System",
                        $"Dear {model.SupervisorName},\n\n" +
                        "Welcome to the Training Management System (TMS)! We are delighted to have you join us.\n\n" +
                        "Here are your account details:\n" +
                        $"- **User ID:** {newUserId}\n" +
                        $"- **Password:** {model.SupervisorPassword}\n\n" +
                        "If you have any questions or need assistance, please do not hesitate to contact our support team.\n\n" +
                        "Best regards,\n" +
                        "The TMS Team"
                    );
                }

                // Redirect to appropriate action after adding supervisor
                return RedirectToAction("Dashboard", "Admin");
            }

            // If model state is not valid, return view with errors
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public ActionResult AddCompanySupervisor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompanySupervisor(AddCompanySupervisorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Map ViewModel data to the User entity
                var user = new Users
                {
                    CompanyName = model.CompanyName,
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Roles = UserRole.CompanySupervisor
                };

                // Save to database
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            // If model state is not valid, return the view with errors
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
