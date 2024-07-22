using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRChatRoom.Server.Models
{
    [Table("SeenMessageLog", Schema = "Chats")]
    public class SeenMessageLog : Entity
    {
        public long ChatRoomId { get; private set; }
        public long ClientId { get;private set; }
        public virtual ChatRoom ChatRoom { get; set; }
        public SeenMessageLog(long chatRoomId, long clientId)
        {
            ChatRoomId = chatRoomId;
            ClientId = clientId;
        }

        public SeenMessageLog()
        {
        }
    }
}
