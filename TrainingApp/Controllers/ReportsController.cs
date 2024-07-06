using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;

namespace TrainingApp.Controllers
{
    public class ReportsController : Controller
    {
        TrainingAppDBContext MyDB = new TrainingAppDBContext();

        // GET: Reports
        public ActionResult Index()
        {
            List<Reports> reports = new List<Reports>();
            reports = (from rep in MyDB.Reports select rep).ToList();

            return View(reports);
        }
        public ActionResult GetReportDetails(string ReportID)
        {
            if (string.IsNullOrEmpty(ReportID))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var report = MyDB.Reports.SingleOrDefault(r => r.ReportId == ReportID);
            if (report == null)
            {
                return HttpNotFound();
            }

            return View(report);
        }
        public ActionResult DeleteReport(string ReportId)
        {
            Reports Report = new Reports();
            Report = (from obj in MyDB.Reports
                      where obj.ReportId == ReportId
                      select obj).FirstOrDefault();
            MyDB.Reports.Remove(Report);
            MyDB.SaveChanges();

            return View("Index");
        }
        public ActionResult UpdateReport(string ReportID, Reports reports)
        {
            Reports Report = new Reports();
            Report = (from obj in MyDB.Reports
                      where obj.ReportId == ReportID
                      select obj).FirstOrDefault();
            MyDB.Reports.AddOrUpdate(reports);
            MyDB.SaveChanges();

            return View("Index");

        }
        public ActionResult InsertReport(string OID, string SuperID, string Name, string Title, DateTime Time, string Des = "")
        {
            Reports report = new Reports()
            {
                ReportDateTime = Time,
                ReportTitle = Title,
                ReportName = Name,
                ReportId = OID,
                ReportDescription = Des,
                OwnerId = OID,
                SupervisorID = SuperID
            };
            MyDB.Reports.Add(report);
            MyDB.SaveChanges();
            return View("Index");

        }
    }
}