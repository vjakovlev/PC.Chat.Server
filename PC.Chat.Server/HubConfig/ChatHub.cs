using Microsoft.AspNetCore.SignalR;
using PC.Chat.Server.Models;

namespace PC.Chat.Server.HubConfig
{
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, UserRoomConnection> _connection;

        public ChatHub(IDictionary<string, UserRoomConnection> connection)
        {
            _connection = connection;
        }

        //Connection Lifetime Events
        public override Task OnConnectedAsync()
        {
            Clients.All.SendAsync("OnConnected", "OnConnected is working");
            return base.OnConnectedAsync();     
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (!_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection? userRoomConnection))
            {
                return base.OnDisconnectedAsync(exception);
            }

            Clients.Group(userRoomConnection.Room!)
                .SendAsync("ReceiveMessage", "PC Chat Bot", $"{userRoomConnection.User} has left the group", DateTime.Now);

            _connection.Remove(Context.ConnectionId);
            Groups.RemoveFromGroupAsync(Context.ConnectionId, userRoomConnection.Room!);

            GetAndSendConnectedUser(userRoomConnection.Room!);

            return base.OnDisconnectedAsync(exception);
        }


        public async Task JoinRoom(UserRoomConnection userRoomConnection) 
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userRoomConnection.Room!);

            _connection[Context.ConnectionId] = userRoomConnection;

            await Clients.Group(userRoomConnection.Room!)
                .SendAsync("ReceiveMessage", "PC Chat Bot", $"{userRoomConnection.User} has joined the group", DateTime.Now);

            await GetAndSendConnectedUser(userRoomConnection.Room!);
        }

        public async Task SendMessage(string message)
        {
            if (_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection? userRoomConnection))
            {
                await Clients.Group(userRoomConnection.Room!)
                    .SendAsync("ReceiveMessage", userRoomConnection.User, message, DateTime.Now);
            }
        }

        public async Task NotifyNewUser(UserRoomConnection userRoomConnection)
        {
            await Clients.Others.SendAsync("NewUser", $"{userRoomConnection.User} has joined {userRoomConnection.Room} room");
        }

        public async Task SetTypingTrue(UserRoomConnection userRoomConnection)
        {
            await Clients.Group(userRoomConnection.Room!).SendAsync("TypingTrue", userRoomConnection.User);
        }

        public async Task SetTypingFalse(UserRoomConnection userRoomConnection)
        {
            await Clients.Group(userRoomConnection.Room!).SendAsync("TypingFalse");
        }

        private Task GetAndSendConnectedUser(string room)
        {
            var users = _connection.Values
                .Where(urc => urc.Room == room)
                .Select(urc => urc.User);

            return Clients.Group(room)
                .SendAsync("ConnectedUser", users);
        }


    }
}
