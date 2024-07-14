using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace TrainingApp.Models
{
    public class ChatHub : Hub
    {
        public Task SendMessage(string senderName, string messageText, int receiverId)
        {
            return Clients.All.broadcastMessage(senderName, messageText, receiverId);
        }
    }
}