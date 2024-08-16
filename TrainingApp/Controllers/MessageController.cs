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
        public async Task<ActionResult> GetChatHistory(int receiverId, int pageIndex = 1, int pageSize = 10)
        {
            int senderId = User.Identity.GetUserId<int>();

            var messagesQuery = _context.ChatMessages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderByDescending(m => m.Timestamp);

            var totalMessages = await messagesQuery.CountAsync();
            var messages = await messagesQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    m.SenderName,
                    m.MessageText,
                    m.Timestamp
                })
                .ToListAsync();

            bool hasMoreMessages = totalMessages > pageIndex * pageSize;

            var result = new
            {
                messages,
                hasMoreMessages
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchUsers(string query)
        {
            // Retrieve the current user's ID
            var currentUserId = User.Identity.GetUserId<int>();

            // Fetch users from the database
            var users = _context.Users
                .Where(u => u.Name.Contains(query) && u.Id != currentUserId)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    Roles = u.Roles, // Assuming Roles is a navigation property
                    Avatar = "" // Assuming Avatar is an empty string or a property that holds the avatar URL
                })
                .ToList()
                .Select(u => new
                {
                    u.Id,
                    Name = $"{u.Name} ({string.Join(", ", u.Roles)})",
                    IsOnline = false, // Adjust this if you have a way to determine online status
                    u.Avatar
                })
                .ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendNewMessage(int ReceiverId, string MessageText)
        {
            try
            {
                if (string.IsNullOrEmpty(MessageText))
                {
                    return Json(new { success = false, message = "Message text cannot be empty" });
                }

                int senderId = User.Identity.GetUserId<int>();
                var senderName = User.Identity.GetUserName();

                var message = new Message
                {
                    SenderId = senderId,
                    ReceiverId = ReceiverId,
                    SenderName = senderName,
                    MessageText = MessageText,
                    Timestamp = DateTime.Now
                };

                _context.ChatMessages.Add(message);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log exception details for debugging
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                return Json(new { success = false, message = "An error occurred while sending the message. Please try again later." });
            }
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
                // Fetch UniversitySupervisorID and CompanySupervisorID
                var user = _context.Users
                                    .Where(u => u.Id == userId)
                                    .Select(u => new { u.UniversitySupervisorID, u.CompanySupervisorID })
                                    .FirstOrDefault();

                if (user == null)
                {
                    // Handle the case where the user is not found
                    return HttpNotFound();
                }

                int uniSuperId = (int)user.UniversitySupervisorID;
                int compSuperId = (int)user.CompanySupervisorID;

                // Fetch the supervisors based on IDs
                var supervisors = _context.Users
                                          .Where(u => u.Id == compSuperId || u.Id == uniSuperId || u.Roles == UserRole.Admin)
                                          .ToList() // Fetch data first
                                          .Select(u => new UsersPanelViewModels
                                          {
                                              Id = u.Id,
                                              Name = $"({u.Name}) ({u.Roles})", // String formatting is now safe
                                              ProfilePicturePath = u.ProfilePicturePath,
                                          })
                                          .ToList();

                return View(supervisors);
            }
            if (userRole == UserRole.UniversitySupervisor)
            {
                var companySupervisorIds = _context.Users
                                      .Where(user => user.UniversitySupervisorID == userId)
                                      .Select(user => user.CompanySupervisorID)
                                      .Distinct()
                                      .ToList();

                var relatedUsers = _context.Users
                         .Where(user => companySupervisorIds.Contains(user.Id) || user.Roles == UserRole.Admin)
                         .Select(user => new
                         {
                             user.Id,
                             user.Name,
                             user.Roles,
                             user.ProfilePicturePath
                         })
                         .ToList() // Fetch data into memory
                         .Select(user => new UsersPanelViewModels
                         {
                             Id = user.Id,
                             Name = $"({user.Name}) ({user.Roles})",
                             ProfilePicturePath = user.ProfilePicturePath,

                         })
                         .ToList();

                var trainers = _context.Users
                                      .Where(user => user.UniversitySupervisorID == userId)
                                      .Select(user => new
                                      {
                                          user.Id,
                                          user.Name,
                                          user.Roles
                                      })
                                      .ToList() // Fetch data into memory
                                      .Select(user => new UsersPanelViewModels
                                      {
                                          Id = user.Id,
                                          Name = $"{user.Name} - ({user.Roles})"
                                      })
                                      .ToList();

                var users = trainers.Union(relatedUsers).ToList();

                return View(users);
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
                               ProfilePicturePath = user.ProfilePicturePath,

                               // CompanySupervisorID = user.CompanySupervisorID
                           })
                           .Distinct() // Ensure no duplicate users
                           .ToList();

                var Trainers =
            _context.Users
                              .Where(user => user.CompanySupervisorID == userId || user.Roles == UserRole.Admin)
                              .Select(user => new UsersPanelViewModels
                              {
                                  Id = user.Id,
                                  Name = user.Name,
                                  ProfilePicturePath = user.ProfilePicturePath,

                                  //     CompId = user.CompanySupervisorID,
                              }).ToList();
                var USERS = Trainers.Union(relatedUsers);

                return View(USERS);
            }
            if (userRole == UserRole.Admin)
            {

                var users = _context.Users
                    .Where(user => user.Id != userId)
                    .Select(user => new
                    {
                        user.Id,
                        user.Name,
                        user.Roles,
                        user.ProfilePicturePath
                    })
                    .ToList()
                    .Select(user => new UsersPanelViewModels
                    {
                        Id = user.Id,
                        Name = $"{user.Name} ({user.Roles})",
                        ProfilePicturePath = user.ProfilePicturePath,

                        // CompId = user.CompanySupervisorID,
                    }).ToList();
                return View(users);
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
