using System;

namespace TrainingApp.ViewModels
{
    public class UsersPanelViewModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CompanySupervisorID { get; set; }
        public bool IsOnline { get; set; }
        public string ProfilePicturePath { get; set; }
        public byte[] Avatar { get; set; } // Ensure this is defined as byte[]
        public string AvatarBase64 => Avatar != null ? Convert.ToBase64String(Avatar) : null;

    }
}