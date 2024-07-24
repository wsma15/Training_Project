using Microsoft.AspNet.Identity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainingApp.Models;
using TrainingApp.ViewModels;

namespace TrainingApp.Controllers
{
    public class ProfileController : Controller
    {


        TrainingAppDBContext _context = new TrainingAppDBContext();
        // GET: Profile
        public ActionResult Profile()
        {
            var userId = User.Identity.GetUserId<int>();
            var user = _context.Users
                               .Where(m => m.Id == userId)
                               .Select(m => new UserViewModel
                               {
                                   Id = m.Id,
                                   Name = m.Name,
                                   Email = m.Email,
                                   Password = m.Password,
                                   UniversitySupervisorID = m.UniversitySupervisorID,
                                   CompanySupervisorID = m.CompanySupervisorID,
                                   UniversityID = m.UniversityID,
                                   CompanyID = m.CompanyID,
                                   Roles = m.Roles,
                                   LastLogin=m.LastLogin,
                                   ProfilePicturePath = m.ProfilePicturePath,
                               }).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }

            return View("Profile", user);
        }
        [HttpPost]
        public ActionResult UploadProfilePicture(HttpPostedFileBase profilePicture)
        {
            if (profilePicture != null && profilePicture.ContentLength > 0)
            {
                // Get the current user
                var userId = User.Identity.GetUserId<int>(); // or however you are identifying the current user
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    // Generate a unique file name using the user ID
                    var fileExtension = Path.GetExtension(profilePicture.FileName);
                    var fileName = userId.ToString() + fileExtension;

                    // Define the path to save the file
                    var directoryPath = Server.MapPath("~/Content/Images");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Save the file to the server
                    profilePicture.SaveAs(filePath);

                    // Update the user's profile picture path
                    user.ProfilePicturePath = "/Content/Images/" + fileName;
                    _context.SaveChanges();
                }

                // Redirect back to the profile page
                return RedirectToAction("Profile");
            }
            // Handle the case where the file was not uploaded
            return View("Error"); // or any appropriate error handling
        }
    }
}