using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TrainingApp.Models;
using TrainingApp.ViewModels;
using static TrainingApp.ViewModels.DashboardViewModel;

namespace TrainingApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using (var context = new TrainingAppDBContext())
                {
                    var user = context.Users.FirstOrDefault(a => a.Id.ToString() == model.UserId && a.Password == model.Password);
                    if (user != null)
                        switch (user.Roles)
                        {
                            case UserRole.Admin: await SignInAdmin(user, model.RememberMe); return RedirectToLocal(returnUrl, "Dashboard", "Admin");
                            case UserRole.UniversitySupervisor: await SignInUniversitySupervisor(user, model.RememberMe); return RedirectToLocal(returnUrl, "Dashboard", "UniversitySupervisor");
                            case UserRole.Trainer: await SignInStudent(user, model.RememberMe); return RedirectToLocal(returnUrl, "Dashboard", "Trainers");
                            case UserRole.CompanySupervisor: await SignInCompanySupervisor(user, model.RememberMe); return RedirectToLocal(returnUrl, "Dashboard", "CompanySupervisor");

                        }


                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            catch (Exception ex)
            {
                // Log the error and provide a user-friendly message
                Trace.TraceError("Error occurred during login: {0}", ex.ToString());
                ModelState.AddModelError("", "An error occurred while logging in. Please try again.");
            }

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
        TrainingAppDBContext db = new TrainingAppDBContext();

        [AllowAnonymous]
        public ActionResult Register()
        {
            var viewModel = new CombinedRegistrationViewModel
            {
                UniversitySupervisors = GetUniversitySupervisorsSelectList(),
                CompanySupervisors = GetCompanySupervisorsSelectList()
            };
            return View(viewModel);
        }

        private IEnumerable<SelectListItem> GetUniversitySupervisorsSelectList()
        {
            var supervisors = db.Users
                .Where(u => u.Roles == UserRole.UniversitySupervisor)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name + " ( " + u.UniversityName + " )"
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
                    Text = u.Name + " ( " + u.CompanyName + " )"
                })
                .ToList();
            return supervisors;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterStudent(CombinedRegistrationViewModel model)
        {
        //    return Content(model.StudentViewModel.TrainerPassword.ToString());
            if (ModelState.IsValid)
            {
                try
                {
                    using (var context = new TrainingAppDBContext())
                    {
                        var companyName = context.Users
                            .Where(u => u.Id == model.StudentViewModel.CompanySupervisorID)
                            .Select(u => u.CompanyName)
                            .FirstOrDefault();
                        var uniName = context.Users
                            .Where(u => u.Id == model.StudentViewModel.UniversitySupervisorID)
                            .Select(u => u.UniversityName)
                            .FirstOrDefault();

                        var user = new Users
                        {
                            Name = model.StudentViewModel.TrainerName,
                            Email = model.StudentViewModel.TrainerEmail,
                            Password = model.StudentViewModel.TrainerPassword,
                            UniversitySupervisorID = model.StudentViewModel.UniversitySupervisorID,
                            CompanySupervisorID = model.StudentViewModel.CompanySupervisorID,
                            CompanyName = companyName,
                            Roles = UserRole.Trainer,
                            UniversityName = uniName,
                        };

                        context.Users.Add(user);
                        context.SaveChanges();
                    }

                    TempData["SuccessMessage"] = "Student registered successfully!";
                    return RedirectToAction("RegistrationSuccess");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while registering the student.");
                }
            }

            var combinedModel = new CombinedRegistrationViewModel
            {
                StudentViewModel = model.StudentViewModel,
                CompanySupervisorViewModel = new AddCompanySupervisorViewModel(),
                UniversitySupervisorViewModel = new AddSupervisorViewModel()
            };
            combinedModel.UniversitySupervisors = GetUniversitySupervisorsSelectList();
            combinedModel.CompanySupervisors = GetCompanySupervisorsSelectList();
            return View("Register", combinedModel);
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
                            Name = model.CompanySupervisorViewModel.FullName,
                            Email = model.CompanySupervisorViewModel.Email,
                            Password = model.CompanySupervisorViewModel.Password,
                            CompanyName = model.CompanySupervisorViewModel.CompanyName,
                            Roles = UserRole.CompanySupervisor
                        };

                        context.Users.Add(user);
                        context.SaveChanges();
                    }

                    TempData["SuccessMessage"] = "Company Supervisor registered successfully!";
                    return RedirectToAction("RegistrationSuccess");
                }
                catch (Exception ex)
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
                            Name = model.UniversitySupervisorViewModel.FullName,
                            Email = model.UniversitySupervisorViewModel.SupervisorEmail,
                            Password = model.UniversitySupervisorViewModel.SupervisorPassword,
                            UniversityName = model.UniversitySupervisorViewModel.UniversityName,
                            Roles = UserRole.UniversitySupervisor
                        };

                        context.Users.Add(user);
                        context.SaveChanges();
                    }

                    TempData["SuccessMessage"] = "University Supervisor registered successfully!";
                    return RedirectToAction("RegistrationSuccess");
                }
                catch (Exception ex)
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

        public ActionResult RegistrationSuccess()
        {
            return View();
        }








        //
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
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