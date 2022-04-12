using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Network.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        static public string receivingMessage = "You received message!";

        static public Dictionary<string, List<string>> users = new Dictionary<string, List<string>>();

        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userEmail = Context.User.Identity.Name;
            
            if (!users.ContainsKey(userEmail))
            {
                users.Add(userEmail, new List<string>());
            }

            if(!users[userEmail].Contains(connectionId))
            {
                users[userEmail].Add(connectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var userEmail = Context.User.Identity.Name;

            if (!users.ContainsKey(userEmail))
            {
                return base.OnDisconnectedAsync(exception);
            }

            if (users[userEmail].Contains(connectionId))
            {
                users[userEmail].Remove(connectionId);
            }

            if(users[userEmail].Count == 0)
            {
                users.Remove(userEmail);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
