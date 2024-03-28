namespace PC.Chat.Server.Services
{
    public interface INotificationService
    {
        Task PingAllClients();
    }
}
