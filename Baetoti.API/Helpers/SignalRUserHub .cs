using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Baetoti.API.Helpers
{
    public class UserHub : Hub
    {
        public async Task UserConnected(string userID)
        {
            // Broadcast the connection status to all clients
            await Clients.All.SendAsync("UserConnected", userID);
        }

        public async Task UserDisconnected(string userID)
        {
            // Broadcast the disconnection status to all clients
            await Clients.All.SendAsync("UserDisconnected", userID);
        }

        public async Task<bool> IsUserOnline(string userID)
        {
            // Check if the user is connected
            Task task = Clients.Caller.SendAsync("IsUserOnlineResult", userID, Context.ConnectionId);
            await task;
            return task.Status == TaskStatus.RanToCompletion;
        }
    }
}
