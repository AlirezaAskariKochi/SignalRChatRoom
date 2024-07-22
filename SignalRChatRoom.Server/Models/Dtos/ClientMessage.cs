namespace SignalRChatRoom.Server.Models.Dtos
{
    public class ClientMessage
    {
        public long Id { get; set; }
        public string ReceiverClientUsernameOrGroupName { get; set; }
        public string SenderClientUsername { get; set; }
        public string Message { get; set; }
        public bool IsGroupMessage { get; set; } = false;
        public bool IsSenderReceiver { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public DateTime? SeenDate { get; set;}
        public bool Seen { get; set;}
        public int SeenCount { get; set;}
        public MessageType MessageType { get; set; }
        public List<long>? SecretClientIds{ get; set; }
    }
}
