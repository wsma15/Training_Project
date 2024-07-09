using System;
using System.Linq;
using System.Web.Mvc;
using TrainingApp.Models;
using TrainingApp.ViewModels;

namespace TrainingApp.Controllers
{
    public class MessageController : Controller
    {
        private readonly TrainingAppDBContext _context = new TrainingAppDBContext();

        [HttpGet]
        public ActionResult Inbox(int userId)
        {
            var messages = _context.Messages
                .Where(m => m.ReceiverId == userId)
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
        public ActionResult SendMessage()
        {
            var users = _context.Users
                .Where(u => u.Roles == UserRole.UniversitySupervisor || u.Roles == UserRole.CompanySupervisor || u.Roles == UserRole.Trainer)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Name
                })
                .ToList();

            ViewBag.Users = users;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage(MessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var message = new MessageViewModel
                {
                    Id = model.Id,
                    SenderId
                     = model.SenderId,
                    ReceiverId = model.ReceiverId,
                    MessageText = model.MessageText,
                    Timestamp = DateTime.Now
                };

                _context.Messages.Add(message);
                _context.SaveChanges();

                return RedirectToAction("Inbox", new { userId = model.SenderId });
            }

            return View(model);
        }
    }
}
