using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrainingApp.Models;
using TrainingApp.ViewModels;

namespace TrainingApp.Controllers
{
    public class MessageController : Controller
    {
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();

        [HttpGet]
        public async Task<ActionResult> GetChatHistory(int receiverId)
        {
            int senderId = User.Identity.GetUserId<int>(); // Assuming you are using ASP.NET Identity
            var messages = await _context.ChatMessages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    m.SenderName,
                    m.MessageText,
                    m.Timestamp
                })
                .ToListAsync();

            return Json(messages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> SendNewMessage(int ReceiverId, string MessageText)
        {
            int senderId = User.Identity.GetUserId<int>(); // Assuming you are using ASP.NET Identity
            var senderName = User.Identity.GetUserName();

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = ReceiverId,
                SenderName = senderName,
                MessageText = MessageText,
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Return an empty result or any specific result if needed
            return Json(new { success = true });
        }

        [HttpGet]
        public ActionResult Inbox(int userId)
        {
            var userRole = _context.Users.Where(u => u.Id == userId).Select(u => u.Roles).FirstOrDefault();
            IQueryable<Message> messagesQuery = _context.ChatMessages;

            if (userRole == UserRole.UniversitySupervisor)
            {
                var studentIds = _context.Users.Where(u => u.UniversitySupervisorID == userId).Select(u => u.Id).ToList();
                messagesQuery = messagesQuery.Where(m => studentIds.Contains(m.SenderId) || m.ReceiverId == userId);
            }
            else if (userRole == UserRole.CompanySupervisor)
            {
                var studentIds = _context.Users.Where(u => u.CompanySupervisorID == userId).Select(u => u.Id).ToList();
                messagesQuery = messagesQuery.Where(m => studentIds.Contains(m.SenderId) || m.ReceiverId == userId);
            }
            else if (userRole == UserRole.Trainer)
            {
                messagesQuery = messagesQuery.Where(m => m.SenderId == userId || m.ReceiverId == userId);
            }

            var messages = messagesQuery
                .Select(m => new
                {
                    m.SenderId,
                    m.ReceiverId,
                    m.MessageText,
                    m.Timestamp,
                    SenderName = _context.Users.FirstOrDefault(u => u.Id == m.SenderId).Name,
                    ReceiverName = _context.Users.FirstOrDefault(u => u.Id == m.ReceiverId).Name
                })
                .ToList()
                .Select(m => new MessageViewModel
                {
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    SenderName = m.SenderName,
                    ReceiverName = m.ReceiverName,
                    MessageText = m.MessageText,
                    Timestamp = m.Timestamp
                })
                .ToList();

            if (userRole == UserRole.Trainer)
            {
                int UniSuperId =
                    (int)(from u in _context.Users where (userId == u.Id) select u.UniversitySupervisorID).FirstOrDefault();

                int CompSuperId =
                    (int)(from u in _context.Users where (userId == u.Id) select u.CompanySupervisorID).FirstOrDefault();

                var supervisors =

                _context.Users
                                  .Where(user => user.Id == CompSuperId || user.Id == UniSuperId)
                                  .Select(user => new UsersPanelViewModels
                                  {
                                      Id = user.Id,
                                      Name = user.Name
                                  })
                                  .ToList();
                return View(supervisors);
            }
            if (userRole == UserRole.UniversitySupervisor)
            {
                var companySupervisorIds = _context.Users
                                  .Where(user => user.UniversitySupervisorID == userId)
                                  .Select(user => user.CompanySupervisorID)
                                  .Distinct() // Ensure no duplicate CompanySupervisorIDs
                                  .ToList();


                var relatedUsers = _context.Users
                           .Where(user => companySupervisorIds.Contains(user.Id))
                           .Select(user => new UsersPanelViewModels
                           {
                               Id = user.Id,
                               Name = user.Name,
                               // CompanySupervisorID = user.CompanySupervisorID
                           })
                           .Distinct() // Ensure no duplicate users
                           .ToList();

                var Trainers =
                _context.Users
                                  .Where(user => user.UniversitySupervisorID == userId)
                                  .Select(user => new UsersPanelViewModels
                                  {
                                      Id = user.Id,
                                      Name = user.Name,
                                      //     CompId = user.CompanySupervisorID,
                                  })
                                  .ToList();
                var USERS = Trainers.Union(relatedUsers);
                return View(USERS);
            }
            if (userRole == UserRole.CompanySupervisor)
            {

                var UniSupervisorIds = _context.Users
                     .Where(user => user.CompanySupervisorID == userId)
                     .Select(user => user.UniversitySupervisorID)
                     .Distinct() // Ensure no duplicate CompanySupervisorIDs
                     .ToList();


                var relatedUsers = _context.Users
                           .Where(user => UniSupervisorIds.Contains(user.Id))
                           .Select(user => new UsersPanelViewModels
                           {
                               Id = user.Id,
                               Name = user.Name,
                               // CompanySupervisorID = user.CompanySupervisorID
                           })
                           .Distinct() // Ensure no duplicate users
                           .ToList();

                var Trainers =
            _context.Users
                              .Where(user => user.CompanySupervisorID == userId)
                              .Select(user => new UsersPanelViewModels
                              {
                                  Id = user.Id,
                                  Name = user.Name,
                                  //     CompId = user.CompanySupervisorID,
                              }).ToList();
                var USERS = Trainers.Union(relatedUsers);

                return View(USERS);
            }
            if (userRole == UserRole.Admin)
            {

                var Users =
           _context.Users.
           Where(user => user.Id != userId)
                             .Select(user => new UsersPanelViewModels
                             {
                                 Id = user.Id,
                                 Name = user.Name,
                                 //     CompId = user.CompanySupervisorID,
                             }).ToList();
                return View(Users);
            }

            else
                return View();
        }

        [HttpGet]
        public ActionResult SendMessageForm(int? receiverId = null)
        {
            var currentUser = _context.Users.Find(User.Identity.GetUserId<int>());
            IQueryable<Users> usersQuery = _context.Users;

            if (currentUser.Roles == UserRole.UniversitySupervisor)
            {
                usersQuery = usersQuery.Where(u => u.UniversitySupervisorID == currentUser.Id);
            }
            else if (currentUser.Roles == UserRole.CompanySupervisor)
            {
                usersQuery = usersQuery.Where(u => u.CompanySupervisorID == currentUser.Id);
            }

            var users = usersQuery.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            }).ToList();

            ViewBag.Users = users;
            ViewBag.SelectedReceiverId = receiverId ?? 0;
            return View(new MessageViewModel { SenderId = currentUser.Id });
        }
    }
}
