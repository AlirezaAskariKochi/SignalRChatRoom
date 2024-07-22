using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRChatRoom.Server.Models
{
    [Table("SeenMessageLog", Schema = "Chats")]
    public class SeenMessageLog : Entity
    {
        public long ChatRoomId { get; private set; }
        public DateTime SeenDateTime { get; private set; }
        public long ClientId { get;private set; }
        public virtual ChatRoom ChatRoom { get; set; }
        public SeenMessageLog(long chatRoomId, DateTime seenDateTime, long clientId)
        {
            ChatRoomId = chatRoomId;
            SeenDateTime = seenDateTime;
            ClientId = clientId;
        }

        public SeenMessageLog()
        {
        }
    }
}
