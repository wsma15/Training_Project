using System.Collections.Generic;
using TrainingApp.Models;

namespace TrainingApp.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Student> Students { get; set; }
        public List<Supervisor> Supervisors { get; set; }
    }
}
