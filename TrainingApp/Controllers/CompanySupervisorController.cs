using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;
using TrainingApp.ViewModels;

public class CompanySupervisorController : Controller
{
    private readonly TrainingAppDBContext _context = new TrainingAppDBContext();
    public ActionResult Dashboard()
    {
        int supervisorId = User.Identity.GetUserId<int>(); // Assuming you are using ASP.NET Identity
        var supervisor = _context.Users
            .FirstOrDefault(u => u.Id == supervisorId);

        if (supervisor == null)
        {
            return View();
        }
        int userCount = _context.Users
                   .Where(u => u.CompanySupervisorID == supervisor.Id)
                   .Count();
        var viewModel = new CompanySupervisorDashboardViewModel
        {
            CompanyName = supervisor.CompanyName.ToString(),
            UserCount = userCount
        };



        return View(viewModel);
    }

    /*    // GET: CompanySupervisor/Dashboard
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            int supervisorId = User.Identity.GetUserId<int>(); // Assuming you are using ASP.NET Identity
            var supervisor = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == supervisorId && u.Roles == UserRole.CompanySupervisor);

            if (supervisor == null)
            {
                return View();
            }

            var userCount = await _context.Users
                .Where(u => u.CompanySupervisorID == supervisor.Id)
                .CountAsync();

            var viewModel = new CompanySupervisorDashboardViewModel
            {
                CompanyName = supervisor.CompanyName,
                UserCount = userCount
            };

            return View(viewModel);
        }
    */
}
