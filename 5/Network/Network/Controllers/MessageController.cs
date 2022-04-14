using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using Network.Models;
using Network.Hubs;

namespace Network.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private NetworkContext db;
        private IHubContext<NotificationHub> hubContext;

        public MessageController(NetworkContext db, IHubContext<NotificationHub> hubContext)
        {
            this.db = db;
            this.hubContext = hubContext;
        }

        public IActionResult Index()
        {
            User? user = AccountController.GetUser(User, db);
            if (user == null) return RedirectToAction("Login", "Account");
            return View(user);
        }

        [HttpGet]
        public IActionResult Write()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Write(IFormCollection formCollection)
        {
            User? author = AccountController.GetUser(User, db);
            User?[] addressees = Array.ConvertAll(formCollection["Ids"].ToArray(), item => db.Users.Find(int.Parse(item)));
            string[] emails = addressees.Select(u => u.Email).ToArray();
            
            if(addressees.Length == 0)
            {
                ModelState.AddModelError("", "There should be at least 1 addressee.");
                return View();
            }
            var message = new MessageBuilder(db)
                .SetAuthor(author)
                .SetSubject(formCollection["Subject"])
                .SetBody(formCollection["Body"])
                .AddAddressee(addressees)
                .ToMessage();
            db.SendMessage(message);
            await Send(message.Id.ToString(), emails);
            return View();
        }

        [HttpGet]
        public IActionResult Reply(int id)
        {
            Message? msg = db.Messages.Find(id);
            if (msg == null)
                return Write();
            return View(msg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(IFormCollection formCollection)
        {
            Message? msg = db.Messages.Find(int.Parse(formCollection["MessageToReply"]));
            User? author = AccountController.GetUser(User, db);
            User? addressee = msg.Author;
            var replyTo = new MessageBuilder(db)
                .SetAuthor(author)
                .AddAddressee(addressee)
                .SetSubject(formCollection["Subject"])
                .SetBody(formCollection["Body"])
                .RepliesTo(msg)
                .ToMessage();
            db.SendMessage(replyTo);
            await Send(replyTo.Id.ToString(), new string[] { addressee.Email });
            return RedirectToAction("Index", "Message");
        }

        public async Task Send(string messageId, string[] addresseeEmails)
        {
            foreach (var addresseeEmail in addresseeEmails)
            {
                if (NotificationHub.users.ContainsKey(addresseeEmail))
                    foreach (var connection in NotificationHub.users[addresseeEmail])
                        await hubContext.Groups.AddToGroupAsync(connection, messageId);
            }
            await hubContext.Clients.Group(messageId).SendAsync("Receive", NotificationHub.receivingMessage);
        }
    }
}
