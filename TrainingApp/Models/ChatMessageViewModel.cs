using System;

namespace TrainingApp.ViewModels
{
    public class ChatMessageViewModel
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
