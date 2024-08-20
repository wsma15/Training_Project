using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrainingApp.Models;
using TrainingApp.ViewModels;

namespace TrainingApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private TrainingAppDBContext db = new TrainingAppDBContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        TrainingAppDBContext _dbContext=new TrainingAppDBContext();
        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // Generate a 6-digit OTP
        }
        [HttpGet]
        [AllowAnonymous]
        public JsonResult CheckEmailExists(string email)
        {
            bool emailExists = _dbContext.Users.Any(u => u.Email == email);
            return Json(emailExists, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]

        public ActionResult ConfirmOtp(string email)
        {
            var model = new ConfirmOtpViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmOtp(ConfirmOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || user.OtpCode != model.OtpCode || user.OtpExpiry < DateTime.Now)
            {
                ModelState.AddModelError("", "Invalid OTP or OTP has expired.");
                return View(model);
            }

            // OTP is valid; allow the user to reset their password
            return RedirectToAction("ResetPassword", new { email = model.Email });
        }
        [HttpPost]
        [Authorize(Roles = "NewUser")]
        public ActionResult SetNewUser(DashboardViewModel viewModel)
        {


            var user = db.Users.Find(User.Identity.GetUserId<int>());
            switch (viewModel.UserRole)
            {
                case UserRole.UniversitySupervisor:
                    user.UniversityID = viewModel.UniversityID;
                    break;
                case UserRole.CompanySupervisor:
                    user.CompanyID=viewModel.CompanyID;
                    break;
                case UserRole.Trainer:
                    user.CompanySupervisorID=viewModel.CompanySupervisorID;
                    user.CompanyID = viewModel.CompanySupervisorID;
                    user.UniversityID=viewModel.UniversityID;
                    user.UniversitySupervisorID=viewModel.UniversitySupervisorID;
                    break;
                default:
                    ModelState.AddModelError("", "Invalid role selected.");
                    return View(viewModel);
            }
            user.Roles = viewModel.UserRole;
            db.Users.AddOrUpdate(user);
            db.SaveChanges();
            LogOff();
            // After processing, redirect to another page or return a success message
            return RedirectToAction("Login", "Account"); // Redirect to an appropriate page
        }
        [Authorize(Roles = "NewUser")]
        public ActionResult SetNewUser() {

            var viewModel = new DashboardViewModel
            {
                users = db.Users.ToList(),

                UniversityNames = db.Universities.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.UniversityName + " - " + u.City
                }).ToList(),
                CompaniesNames = db.Companies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CompanyName + " - " + c.City
                }).ToList(),

                CompanySupervisors = db.Users
        .Where(u => u.Roles == UserRole.CompanySupervisor)
        .Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Name + " - " + db.Companies
                .Where(c => c.Id == u.CompanyID)
                .Select(c => c.CompanyName)
                .FirstOrDefault()
        }).ToList(),
                UniSupervisors = db.Users
        .Where(u => u.Roles == UserRole.UniversitySupervisor)
        .Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Name + " - " + db.Universities
                .Where(uni => uni.Id == u.UniversityID)
                .Select(uni => uni.UniversityName)
                .FirstOrDefault()
        }).ToList()
            };
            return View(viewModel); }
        [AllowAnonymous]

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email exists in the database
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null)
                {
                    // Generate a random OTP (for example, a 6-digit code)
                    var otp = new Random().Next(100000, 999999).ToString();

                    // Store the OTP and expiry time (e.g., 5 minutes from now) in the database
                    user.OtpCode = otp;
                    user.OtpExpiry = DateTime.Now.AddMinutes(5);
                    await _dbContext.SaveChangesAsync();

                    // Send the OTP to the user's email (use your email service)
                    // Example: await _emailService.SendOtpEmail(user.Email, otp);
                    MailHelper.SendEmail(
    user.Email,
    "Password Reset OTP for Training Management System",
    $"Dear {user.Name},\n\n" +
    "We received a request to reset your password for the Training Management System (TMS). To proceed with the password reset, please use the following one-time password (OTP):\n\n" +
    $"**{otp}**\n\n" +
    "Enter this OTP on the password reset page to verify your request. Please note that the OTP is valid for a short period of time and will expire soon.\n\n" +
    "If you did not request a password reset or if you have any questions, please contact our support team immediately.\n\n" +
    "Best regards,\n" +
    "The TMS Team"
);
                    TempData["Message"] = "An OTP has been sent to your email address.";
                    return RedirectToAction("ConfirmOtp", new { email = model.Email });
                }
                else
                {
                    // If the email is not found, add a model error
                    ModelState.AddModelError("", "The email address does not exist.");
                }
            }
            return View(model);
        }

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewData.Clear();
            ViewBag.InvalidLogin = null;

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            try
            {
                using (var context = new TrainingAppDBContext())
                {
                    var user = context.Users.FirstOrDefault(a => a.Email.ToUpper() == model.Email.ToUpper() && a.Password == model.Password);

                    if (user != null)
                    {
                        // Check if email is confirmed
                        if (!user.ConfirmedEmail)
                        {
                            // Generate a new email confirmation token
                            user.EmailConfirmationToken = Guid.NewGuid().ToString();

                            // Resend confirmation email
                            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token = user.EmailConfirmationToken, email = user.Email }, Request.Url.Scheme);
                            SendConfirmationEmail(user.Email, confirmationLink);

                            context.Users.AddOrUpdate(user);
                            await context.SaveChangesAsync();

                            ViewBag.EmailNotConfirm = "Your email is not confirmed." + "<br/>" + "A new confirmation email has been sent.";
                            return View(model);
                        }

                        user.LastLogin = DateTime.Now;
                        context.Users.AddOrUpdate(user);
                        await context.SaveChangesAsync();

                        switch (user.Roles)
                        {
                            case UserRole.Admin:
                                await SignInAdmin(user, model.RememberMe);
                                return RedirectToLocal(returnUrl, "Dashboard", "Admin");

                            case UserRole.UniversitySupervisor:
                                await SignInUniversitySupervisor(user, model.RememberMe);
                                return RedirectToLocal(returnUrl, "Dashboard", "UniversitySupervisor");

                            case UserRole.Trainer:
                                await SignInStudent(user, model.RememberMe);
                                return RedirectToLocal(returnUrl, "Dashboard", "Trainers");

                            case UserRole.CompanySupervisor:
                                await SignInCompanySupervisor(user, model.RememberMe);
                                return RedirectToLocal(returnUrl, "Dashboard", "CompanySupervisor");
                            case UserRole.NewUser:
                                await SignInNewUser(user, model.RememberMe);
                                return RedirectToLocal(returnUrl, "SetNewUser", "Account");

                            default:
                                ModelState.AddModelError("", "Invalid role.");
                                break;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid login attempt.");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error occurred during login: {0}", ex.ToString());
                ModelState.AddModelError("", "An error occurred while logging in. Please try again.");
            }

            ViewBag.InvalidLogin = "Invalid login attempt.";
            return View(model);
        }

        private async Task SignInAdmin(Users admin, bool rememberMe)
        {
            var identity = new ClaimsIdentity(new[] {
        new Claim(ClaimTypes.Role, "Admin"),
        new Claim(ClaimTypes.Name, admin.Name),
        new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString())
    }, DefaultAuthenticationTypes.ApplicationCookie);

            await SignInAsync(identity, rememberMe);
        }
        private async Task SignInUniversitySupervisor(Users UniSupervisor, bool rememberMe)
        {

            var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Role, "UniversitySupervisor"),
        new Claim(ClaimTypes.Name, UniSupervisor.Name),
        new Claim(ClaimTypes.NameIdentifier, UniSupervisor.Id.ToString())
    }, DefaultAuthenticationTypes.ApplicationCookie);

            await SignInAsync(identity, rememberMe);
        }
        private async Task SignInCompanySupervisor(Users UniSupervisor, bool rememberMe)
        {
            var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Role, "CompanySupervisor"),
        new Claim(ClaimTypes.Name, UniSupervisor.Name),
        new Claim(ClaimTypes.NameIdentifier, UniSupervisor.Id.ToString())
    }, DefaultAuthenticationTypes.ApplicationCookie);

            await SignInAsync(identity, rememberMe);
        }
        private async Task SignInNewUser(Users newuser, bool rememberMe)
        {
            var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Role, "NewUser"),
        new Claim(ClaimTypes.Name, newuser.Name),
        new Claim(ClaimTypes.NameIdentifier, newuser.Id.ToString())
    }, DefaultAuthenticationTypes.ApplicationCookie);

            await SignInAsync(identity, rememberMe);
        }

        private async Task SignInStudent(Users Trainer, bool rememberMe)
        {
            var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Role, "Trainer"),
        new Claim(ClaimTypes.Name, Trainer.Name),
        new Claim(ClaimTypes.NameIdentifier, Trainer.Id.ToString())
    }, DefaultAuthenticationTypes.ApplicationCookie);

            await SignInAsync(identity, rememberMe);
        }
        private async Task SignInCompany(Users CompSupervisor, bool rememberMe)
        {
            var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Role, "CompanySupervisor"),
        new Claim(ClaimTypes.Name, CompSupervisor.Name),
        new Claim(ClaimTypes.NameIdentifier, CompSupervisor.Id.ToString())
    }, DefaultAuthenticationTypes.ApplicationCookie);

            await SignInAsync(identity, rememberMe);
        }

        private Task SignInAsync(ClaimsIdentity identity, bool rememberMe)
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, identity);
            return Task.CompletedTask;
        }


        private ActionResult RedirectToLocal(string returnUrl, string actionName, string controllerName)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(actionName, controllerName);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        private IEnumerable<SelectListItem> GetUniversitySupervisorsSelectList()
        {
            var supervisors = db.Users
                .Where(u => u.Roles == UserRole.UniversitySupervisor)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name + " ( " + u.UniversityID + " )"
                })
                .ToList();
            return supervisors;
        }

        private IEnumerable<SelectListItem> GetCompanySupervisorsSelectList()
        {
            var supervisors = db.Users
                .Where(u => u.Roles == UserRole.CompanySupervisor)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name + " ( " + u.CompanyID + " )"
                })
                .ToList();
            return supervisors;
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            var viewModel = new RegisterViewModel
            {
                Roles = new List<SelectListItem>
        {
            new SelectListItem { Value = UserRole.Trainer.ToString(), Text = "Trainer" },
            new SelectListItem { Value = UserRole.UniversitySupervisor.ToString(), Text = "University Supervisor" },
            new SelectListItem { Value = UserRole.CompanySupervisor.ToString(), Text = "Company Supervisor" }
        },
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = null;

                try
                {
                    using (var context = new TrainingAppDBContext())
                    {
                        // Check if the email already exists in the database
                        var existingUser = context.Users.FirstOrDefault(u => u.Email == model.Email);
                        if (existingUser != null)
                        {
                            ModelState.AddModelError("Email", "A user with this email address already exists.");
                            return View(model); // Return the view with validation errors
                        }

                        // Generate a unique token for email confirmation
                        var emailConfirmationToken = Guid.NewGuid().ToString();

                        var user = new Users
                        {
                            Name = model.FullName,
                            Email = model.Email,
                            Password = model.Password,
                            Roles = UserRole.NewUser, // Assign the appropriate role
                            AddedBy = "Its Self",
                            ConfirmedEmail = false,
                            EmailConfirmationToken = emailConfirmationToken // Store the token in the database
                        };

                        context.Users.Add(user);
                        context.SaveChanges();

                        // Send confirmation email
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token = emailConfirmationToken, email = user.Email }, Request.Url.Scheme);
                        SendConfirmationEmail(user.Email, confirmationLink); // You need to implement this method

                        TempData["SuccessMessage"] = "Registered successfully! Please check your email to confirm your account.";
                        return View(model); // Redirect to a confirmation page or return the view
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    ModelState.AddModelError("", "An error occurred while processing your request."+ex.ToString());
                }
            }
            return View(model); // Return the view with validation errors
        }
        private void SendConfirmationEmail(string email, string confirmationLink)
        {
            var subject = "Confirm your email address";
            var body = $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";

         
            MailHelper.SendEmail(email, subject, body);
        }
        [AllowAnonymous]

        public ActionResult ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Error", "Home"); // Handle invalid tokens or missing data
            }

            using (var context = new TrainingAppDBContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email && u.EmailConfirmationToken == token);
                if (user != null)
                {
                    user.ConfirmedEmail = true;
                    user.EmailConfirmationToken = null; // Clear the token after confirmation
                    context.SaveChanges();

                    ViewBag.Message = "Your email has been confirmed. You can now log in.";
                    return View("Login"); // Redirect to a confirmation success page
                }
            }

            return RedirectToAction("Error", "Home"); // Handle invalid confirmation attempts
        }

        private string FormatValidationErrors(ModelStateDictionary modelState)
        {
            var errors = modelState
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            return string.Join("; ", errors);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCompanySupervisor(CombinedRegistrationViewModel model)
        {
            //if (ModelState.IsValid)
            {
                try
                {
                    using (var context = new TrainingAppDBContext())
                    {
                        var user = new Users
                        {
                            Name = model.CompanySupervisorViewModel.Name,
                            Email = model.CompanySupervisorViewModel.Email,
                            Password = model.CompanySupervisorViewModel.Password,
                            CompanyID = model.CompanySupervisorViewModel.CompanyID,
                            Roles = UserRole.CompanySupervisor
                        };

                        context.Users.Add(user);
                        context.SaveChanges();
                        MailHelper.SendEmail(
user.Email,
"Welcome to the Training Management System",
$"Dear {user.Name} Supervisor,\n\n" +
"Welcome to the Training Management System (TMS)! We are delighted to have you join us.\n\n" +
"Here are your account details:\n" +
$"- **User ID:** {user.Id}\n" +
$"- **Password:** {user.Password}\n\n" +
"If you have any questions or need assistance, please do not hesitate to contact our support team.\n\n" +
"Best regards,\n" +
"The TMS Team"
);
                    }

                    TempData["SuccessMessage"] = "Company Supervisor registered successfully!";
                    return RedirectToAction("RegistrationSuccess");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while registering the company supervisor.");
                }
            }

            var combinedModel = new CombinedRegistrationViewModel
            {
                StudentViewModel = new AddTrainerViewModel(),
                CompanySupervisorViewModel = model.CompanySupervisorViewModel,
                UniversitySupervisorViewModel = new AddSupervisorViewModel()
            };
            combinedModel.UniversitySupervisors = GetUniversitySupervisorsSelectList();
            combinedModel.CompanySupervisors = GetCompanySupervisorsSelectList();
            return View("Register", combinedModel);
        }
        [AllowAnonymous]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUniversitySupervisor(CombinedRegistrationViewModel model)
        {
            //   if (ModelState.IsValid)
            {
                try
                {
                    using (var context = new TrainingAppDBContext())
                    {
                        var user = new Users
                        {
                            Name = model.UniversitySupervisorViewModel.SupervisorName,
                            Email = model.UniversitySupervisorViewModel.SupervisorEmail,
                            Password = model.UniversitySupervisorViewModel.SupervisorPassword,
                            UniversityID = model.UniversitySupervisorViewModel.UniversityID,
                            Roles = UserRole.UniversitySupervisor
                        };

                        context.Users.Add(user);
                        context.SaveChanges();
                        MailHelper.SendEmail(
user.Email,
"Welcome to the Training Management System",
$"Dear {user.Name} Supervisor,\n\n" +
"Welcome to the Training Management System (TMS)! We are delighted to have you join us.\n\n" +
"Here are your account details:\n" +
$"- **User ID:** {user.Id}\n" +
$"- **Password:** {user.Password}\n\n" +
"If you have any questions or need assistance, please do not hesitate to contact our support team.\n\n" +
"Best regards,\n" +
"The TMS Team"
);
                    }

                    TempData["SuccessMessage"] = "University Supervisor registered successfully!";
                    return RedirectToAction("RegistrationSuccess");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while registering the university supervisor.");
                }
            }

            var combinedModel = new CombinedRegistrationViewModel
            {
                StudentViewModel = new AddTrainerViewModel(),
                CompanySupervisorViewModel = new AddCompanySupervisorViewModel(),
                UniversitySupervisorViewModel = model.UniversitySupervisorViewModel
            };
            combinedModel.UniversitySupervisors = GetUniversitySupervisorsSelectList();
            combinedModel.CompanySupervisors = GetCompanySupervisorsSelectList();
            return View("Register", combinedModel);
        }
        [AllowAnonymous]

        public ActionResult RegistrationSuccess()
        {
            return View();
        }



        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPassword(string email)
        {
            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email address.");
                return View(model);
            }

            // Hash the new password (you should use a password hashing mechanism like Identity or BCrypt)
            user.Password = (model.Password);
            user.OtpCode = null; // Clear the OTP code once the password is reset
            

            await _dbContext.SaveChangesAsync();

            TempData["Message"] = "Your password has been reset successfully. You can now log in with the new password.";
            return RedirectToAction("Login", "Account");
        }
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}