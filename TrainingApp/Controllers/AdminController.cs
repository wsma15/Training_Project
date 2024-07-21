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
                NewUsers = _context.Users.Where(
                    super => super.Roles == UserRole.NewUser
                    ).ToList(),
                UniversityNames= _context.Universities.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UniversityName + " - " + u.City // Display company name and supervisor name
                }).ToList(),
                CompaniesNames= _context.Companies.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.CompanyName + " - " + u.City // Display company name and supervisor name
                }).ToList()

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
                                        Text = u.CompanyID + " - " + u.Name // Display company name and supervisor name
                                    })
                                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddStudent(DashboardViewModel model)
        {
            if (ModelState.IsValid)
            {

                using (var context = new TrainingAppDBContext())
                {
                    // Retrieve the company name based on the selected supervisor ID
                    var companyName = context.Users
                                             .Where(u => u.Id == model.addTrainerViewModel.CompanySupervisorID)
                                             .Select(u => u.CompanyID)
                                             .FirstOrDefault();
                    var UniName = context.Users
                                 .Where(u => u.Id == model.addTrainerViewModel.UniversitySupervisorID)
                                 .Select(u => u.UniversityID)
                                 .FirstOrDefault();
                    var user = new Users
                    {
                        Name = model.addTrainerViewModel.TrainerName,
                        Email = model.addTrainerViewModel.TrainerEmail,
                        Password = model.addTrainerViewModel.TrainerPassword,
                        UniversitySupervisorID = model.addTrainerViewModel.UniversitySupervisorID,
                        CompanySupervisorID = model.addTrainerViewModel.CompanySupervisorID,
                        CompanyID = companyName, // Save the company name
                        Roles = UserRole.Trainer,
                        UniversityID = UniName,
                    };

                    context.Users.Add(user);
                    context.SaveChanges();
/*                    MailHelper.SendEmail(
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
*/                }

                // Redirect to the admin dashboard after adding the student
                return RedirectToAction("Dashboard", "Admin");
            }

            // If the model state is not valid, return the view with validation errors
            return View(model);
        }
        public string GetUniName(int id)
        {
            return (from name in _context.Universities where name.Id == id select name.UniversityName).FirstOrDefault();


        }
  
        [HttpPost]
       // [HttpGet]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AddUniversitySupervisor(DashboardViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new TrainingAppDBContext())
                {
                    var newUser = new Users
                    {
                        Name = model.addSupervisorViewModel.SupervisorName,
                        Email = model.addSupervisorViewModel.SupervisorEmail,
                        Password = model.addSupervisorViewModel.SupervisorPassword, // Ensure you are hashing passwords in a real application
                        UniversityID = model.addSupervisorViewModel.UniversityID,
                        Roles = UserRole.UniversitySupervisor
                    };

                    context.Users.Add(newUser);
                    context.SaveChanges();

                    return RedirectToAction("Dashboard", "Admin");
                }
            }
            AddSupervisorViewModel mod = model.addSupervisorViewModel;

            // If model state is not valid, return view with errors
            return View(model.addSupervisorViewModel);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult AddCompanySupervisor()
        {
            // Return the view with the appropriate view model
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AddCompanySupervisor(DashboardViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    CompanyID = model.addCompanySupervisorViewModel.CompanyID,
                    Name = model.addCompanySupervisorViewModel.Name,
                    Email = model.addCompanySupervisorViewModel.Email,
                    Password = model.addCompanySupervisorViewModel.Password, // Ensure you hash the password
                    
                    Roles = UserRole.CompanySupervisor
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            // Return the view with the errors
            return View();
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
