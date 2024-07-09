using System.Collections.Generic;
using System.Linq;
using TrainingApp.Models;

namespace TrainingApp.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Users> Students { get; set; }
        public List<Users> Supervisors { get; set; }
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();

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
