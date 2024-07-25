﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrainingApp.Models;
using TrainingApp.ViewModels;
using PagedList;

namespace TrainingApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Dashboard(int page = 1)
        {
            int pageSize = 10; // Number of items per page

            var viewModel = new DashboardViewModel
            {
                Trainers = _context.Users
                    .Where(u => u.Roles == UserRole.Trainer)
                    .OrderBy(u => u.Id)
                    .ToPagedList(page, pageSize),

                UniversitySupervisors = _context.Users
                    .Where(u => u.Roles == UserRole.UniversitySupervisor)
                    .OrderBy(u => u.Id)
                    .ToPagedList(page, pageSize),

                CompanySupervisors = _context.Users
                    .Where(u => u.Roles == UserRole.CompanySupervisor)
                    .OrderBy(u => u.Id)
                    .ToPagedList(page, pageSize),

                NewUsers = _context.Users
                    .Where(u => u.Roles == UserRole.NewUser)
                    .OrderBy(u => u.Id)
                    .ToPagedList(page, pageSize),

                UniversityNames = _context.Universities.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UniversityName + " - " + u.City
                }).ToList(),

                CompaniesNames = _context.Companies.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.CompanyName + " - " + u.City
                }).ToList(),

                UniSupervisors = _context.Users
                    .Where(u => u.Roles == UserRole.UniversitySupervisor)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Name + " - " + _context.Universities
                            .Where(name => name.Id == u.UniversityID)
                            .Select(name => name.UniversityName)
                            .FirstOrDefault()
                    }).ToList(),
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Dashboard");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "User deleted successfully";
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public JsonResult GetSupervisorsByUniversity(int universityId)
        {
            var supervisors = _context.Users
                .Where(u => u.Roles == UserRole.UniversitySupervisor && u.UniversityID == universityId)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name + " - " + _context.Universities
                        .Where(name => name.Id == u.UniversityID)
                        .Select(name => name.UniversityName)
                        .FirstOrDefault()
                })
                .ToList();

            return Json(supervisors, JsonRequestBehavior.AllowGet);
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

                CompanySupervisors = _context.Users
                    .Where(u => u.Roles == UserRole.CompanySupervisor)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.CompanyID + " - " + u.Name
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
                var companyName = _context.Users
                    .Where(u => u.Id == model.addTrainerViewModel.CompanySupervisorID)
                    .Select(u => u.CompanyID)
                    .FirstOrDefault();
                var uniName = _context.Users
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
                    CompanyID = companyName,
                    Roles = UserRole.Trainer,
                    UniversityID = uniName,
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                // Uncomment if you want to send a welcome email
                /* MailHelper.SendEmail(
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
                ); */

                return RedirectToAction("Dashboard");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AddUniversitySupervisor(DashboardViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new Users
                {
                    Name = model.addSupervisorViewModel.SupervisorName,
                    Email = model.addSupervisorViewModel.SupervisorEmail,
                    Password = model.addSupervisorViewModel.SupervisorPassword,
                    UniversityID = model.addSupervisorViewModel.UniversityID,
                    Roles = UserRole.UniversitySupervisor
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            return View(model.addSupervisorViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult AddCompanySupervisor()
        {
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
                    Password = model.addCompanySupervisorViewModel.Password,
                    Roles = UserRole.CompanySupervisor
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }

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
