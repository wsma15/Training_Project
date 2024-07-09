using System.Collections.Generic;
using System.Linq;
using TrainingApp.Models;

namespace TrainingApp.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Users> Students { get; set; }
        public List<Users> UniversitySupervisors { get; set; }
        public List<Users> CompanySupervisors { get; set; }
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();

        public string GetUniName(string supervisorId)
        {
            // Assuming you have a method to retrieve supervisor details from the database
            string supervisor = (from name in _context.Users where name.Roles == UserRole.UniversitySupervisor && name.Id.ToString() == supervisorId select name.UniversityName).FirstOrDefault();

            if (supervisor != null)
            {
                return supervisor;
            }
            else
            {
                return "University not found"; // Or handle the case when supervisor is not found
            }
        }
        public string GetCompanyName(string supervisorId)
        {
            // Assuming you have a method to retrieve supervisor details from the database
            string supervisor = (from name in _context.Users where name.Roles == UserRole.CompanySupervisor && name.Id.ToString() == supervisorId select name.CompanyName).FirstOrDefault();

            if (supervisor != null)
            {
                return supervisor;
            }
            else
            {
                return "Company not found"; // Or handle the case when supervisor is not found
            }
        }
        public string GetSupervisorName(string supervisorId)
        {
            // Assuming you have a method to retrieve supervisor details from the database
            var supervisor = _context.Users.FirstOrDefault(s => s.Id.ToString() == supervisorId);

            if (supervisor != null)
            {
                return supervisor.Name;
            }
            else
            {
                return "Supervisor not found"; // Or handle the case when supervisor is not found
            }
        }

    }
}
