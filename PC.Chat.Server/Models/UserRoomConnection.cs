namespace PC.Chat.Server.Models
{
    public record UserRoomConnection
    {
        public string? User { get; set; }
        public string? Room { get; set; }
    }
}
