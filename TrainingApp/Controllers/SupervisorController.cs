using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    public class SupervisorController : Controller
    {
        private TrainingAppDBContext db = new TrainingAppDBContext();

        // GET: Supervisor/Dashboard
        public ActionResult Dashboard()
        {
            string supervisorId = User.Identity.GetUserId(); // Assuming UserId is the SupervisorID
            var students = db.Students.Where(s => s.SupervisorID == supervisorId).ToList();
            return View(students);
        }

        // GET: Supervisor/StudentReports/5
        public ActionResult StudentReports(string id)
        {
            var reports = db.Reports.Where(r => r.OwnerId == id).ToList();
            return View(reports);
        }
    }
}
