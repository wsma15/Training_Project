﻿using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;
using TrainingApp.ViewModels;

namespace TrainingApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();
        // GET: /Admin/Dashboard
        //  [Authorize(Roles = "Admin")]
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

        public ActionResult AddUniversitySupervisor(AddSupervisorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Perform actions to add supervisor to database
                using (var context = new TrainingAppDBContext())
                {
                    context.Users.Add(new Users
                    {
                        Name = model.SupervisorName,
                        Email = model.SupervisorEmail,
                        Password = model.SupervisorPassword,
                        UniversityName = model.UniversityName, // Assuming you have this property in your Users model
                        Roles = UserRole.UniversitySupervisor // Adjust roles as per your application logic
                    });
                    context.SaveChanges();
                }

                // Redirect to appropriate action after adding supervisor
                return RedirectToAction("Dashboard", "Admin");
            }

            // If model state is not valid, return view with errors
            return View(model);
        }
        [HttpGet]
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
