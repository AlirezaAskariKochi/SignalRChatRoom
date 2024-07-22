namespace SignalRChatRoom.Client.Models
{
   public class ClientMessage
    {
        public string ReceiverClientUsernameOrGroupName { get; set; }
        public string SenderClientUsername { get; set; }
        public string Message { get; set; }
        public bool IsGroupMessage { get; set; } = false;
        public bool IsSenderReceiver { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
