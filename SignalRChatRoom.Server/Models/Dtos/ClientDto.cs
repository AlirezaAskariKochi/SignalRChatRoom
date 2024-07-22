namespace SignalRChatRoom.Server.Models.Dtos
{
    public class ClientDto
    {
        public long Id { get; set; }
        public string ConnectionId { get; set; }
        public string Username { get; set; }
        public string Guid { get; set; }
        public string PersianName { get; set; }
    }
}
