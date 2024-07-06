using System.Collections.Generic;
using System.Linq;
using TrainingApp.Models;

namespace TrainingApp.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Student> Students { get; set; }
        public List<Supervisor> Supervisors { get; set; }
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();

        public string GetSupervisorName(string supervisorId)
        {
            // Assuming you have a method to retrieve supervisor details from the database
            var supervisor = _context.Supervisors.FirstOrDefault(s => s.SupervisorID == supervisorId);

            if (supervisor != null)
            {
                return supervisor.SupervisorName;
            }
            else
            {
                return "Supervisor not found"; // Or handle the case when supervisor is not found
            }
        }

    }
}
