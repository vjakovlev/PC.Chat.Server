using Microsoft.AspNetCore.SignalR;
using PC.Chat.Server.HubConfig;

namespace PC.Chat.Server.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<ChatHub> _chatHub;

        public NotificationService(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
        }

        public async Task PingAllClients() 
        {
            await _chatHub.Clients.All.SendAsync("Ping");
        }
    }
}
