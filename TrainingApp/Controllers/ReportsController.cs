using Microsoft.AspNet.Identity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var reportFile = new ReportFile
                {
                    FileName = Path.GetFileName(file.FileName),
                    ContentType = file.ContentType,
                    OwnerId = User.Identity.GetUserId(),
                    SupervisorID = (from s in _context.Students
                                    where s.StudentID == User.Identity.GetUserId()
                                    select s.SupervisorID).FirstOrDefault()
                };

                using (var reader = new BinaryReader(file.InputStream))
                {
                    reportFile.Content = reader.ReadBytes(file.ContentLength);
                }

                _context.ReportFiles.Add(reportFile);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Please upload a file.");
            return View();
        }
    }
}
