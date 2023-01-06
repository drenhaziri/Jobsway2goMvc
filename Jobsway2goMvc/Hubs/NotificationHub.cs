using Jobsway2goMvc.Data;
using Jobsway2goMvc.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Jobsway2goMvc.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationHub (ApplicationDbContext ctx, IHubContext<NotificationHub> hubContext)
        {
            _context = ctx;
            _hubContext = hubContext;
        }

        public async Task SendNotificationToAll(string message)
        {
            await Clients.All.SendAsync("ReceivedNotification", message);
        }
        public async Task SendNotificationToClient(string message, string username)
        {
            var hubConnections = _context.HubConnections.Where(con => con.UserName == username).ToList();
            foreach (var hubConnection in hubConnections)
            {
                await _hubContext.Clients.Client(hubConnection.ConnectionId).SendAsync("ReceivedPersonalNotification", message, username);
            }
        }
        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("OnConnected");
            return base.OnConnectedAsync();
        }
        public async Task SaveUserConnection(string username)
        {
            var connectionId = Context.ConnectionId;

            HubConnection hubConnection = new HubConnection
            {
                ConnectionId = connectionId,
                UserName = username
            };
            _context.HubConnections.Add(hubConnection);
            await _context.SaveChangesAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var hubConnection = _context.HubConnections.FirstOrDefault(con => con.ConnectionId == Context.ConnectionId);
            if(hubConnection != null)
            {
                _context.HubConnections.Remove(hubConnection);
                 _context.SaveChangesAsync();
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
