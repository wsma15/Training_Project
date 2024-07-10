using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using TrainingApp.Models;
using TrainingApp.ViewModels;

namespace TrainingApp.Controllers
{
    public class MessageController : Controller
    {
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();
        [HttpGet]
        public ActionResult GetChatHistory(int receiverId)
        {
            var userId = User.Identity.GetUserId<int>();

            var chatHistory = _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == userId))
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatMessageViewModel
                {
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    SenderName = _context.Users.FirstOrDefault(u => u.Id == m.SenderId).Name,
                    ReceiverName = _context.Users.FirstOrDefault(u => u.Id == m.ReceiverId).Name,
                    MessageText = m.MessageText,
                    Timestamp = m.Timestamp
                })
                .ToList();

            return Json(chatHistory, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Inbox(int userId)
        {
            var userRole = _context.Users.Where(u => u.Id == userId).Select(u => u.Roles).FirstOrDefault();
            IQueryable<Message> messagesQuery = _context.Messages;

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

            return View(messages);
        }

        [HttpGet]
        public ActionResult SendMessage(int? receiverId = null)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(MessageViewModel model)
        {
            string recivername = (from RName in _context.Users where model.ReceiverId == RName.Id select RName.Name).FirstOrDefault();
            model.ReceiverName = recivername;
            model.SenderId = User.Identity.GetUserId().AsInt();
            model.SenderName = User.Identity.Name;
            /*string 
            return Content(recivername);
            */
            if (!string.IsNullOrEmpty(recivername) && !string.IsNullOrEmpty(model.MessageText))
            {
                var message = new Message
                {
                    SenderId = model.SenderId,
                    ReceiverId = model.ReceiverId,
                    MessageText = model.MessageText,
                    Timestamp = DateTime.Now,

                };

                _context.Messages.Add(message);
                _context.SaveChanges();

                return RedirectToAction("Inbox", new { userId = model.SenderId });
            }
            /*else
            {
                // Assuming ModelState has been populated with errors or other data
                var modelStateValues = ModelState.Values;

                // Convert ModelState.Values to JSON
                var jsonResult = JsonConvert.SerializeObject(modelStateValues, Formatting.Indented);

                // Return JSON result
                return Content(jsonResult, "application/json");
            }*/
            // If model state is not valid, re-populate ViewBag.Users and return the view with the model
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
            return View(model);
        }
    }
}
