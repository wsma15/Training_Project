using Microsoft.AspNet.Identity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpGet]
        [Route("Profile/{userId}")]
        // Profile for a specific user by ID
        public ActionResult UserProfile(int userId)
        {
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
                                   UniversityID = (int)m.UniversityID,
                                   CompanyID = (int)m.CompanyID,
                                   Roles = m.Roles,
                                   LastLogin = m.LastLogin,
                                   ProfilePicturePath = m.ProfilePicturePath,
                                   IsCurrentUser = false
                               }).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }

            return View("Profile", user);
        }

        // Profile for the currently logged-in user
        public ActionResult CurrentProfile()
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
                                   UniversityID = (int)m.UniversityID,
                                   CompanyID = (int)m.CompanyID,
                                   Roles = m.Roles,
                                   LastLogin = m.LastLogin,
                                   ProfilePicturePath = m.ProfilePicturePath,
                                   IsCurrentUser = true
                               }).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }

            return View("Profile", user);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateProfile(string fullName, string email, string password)
        {
            // Retrieve the current user ID
            var userId = User.Identity.GetUserId<int>();

            // Find the user in the database
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Update the user's profile
            user.Name = fullName;
            user.Email = email;

            if (!string.IsNullOrEmpty(password))
            {
                // Hash the password and set it (assuming you use ASP.NET Identity)
                var hasher = new PasswordHasher();
                user.Password = (password);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect back to the profile page with a query string parameter indicating success
            return RedirectToAction("Profile", "Profile", new { userId = userId });
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