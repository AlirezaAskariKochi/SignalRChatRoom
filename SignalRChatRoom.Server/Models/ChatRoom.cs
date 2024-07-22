using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SignalRChatRoom.Server.Models.Enums;

namespace SignalRChatRoom.Server.Models
{
    [Table("ChatRooms", Schema = "Chats")]
    public class ChatRoom : Entity
    {
        public long FromId { get;private set; }
        public long? ToId { get; private set; }
        public long? GroupId { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public ChatType Type { get; private set; }
        public long? ReplyId { get; private set; }
        public long? ForwardId { get; private set; }
        public ICollection<SeenMessageLog> SeenMessageLogs { get;} =new List<SeenMessageLog>();
        public virtual Client FromClient { get; set; }
        public virtual Client? ToClient { get; set; }
        public virtual Group? Group { get; set; }
        public ChatRoom(long fromId, long? toId,long? groupId, string message, ChatType type, long? replyId, long? forwardId)
        {
            FromId = fromId;
            ToId = toId;
            GroupId = groupId;
            Message = message;
            Type = type;
            ReplyId = replyId;
            ForwardId = forwardId;
        }
        public ChatRoom()
        {
                
        }

        public void AddSeenMessageLog(long clientId)
        {
            SeenMessageLogs.Add(new SeenMessageLog(Id,DateTime.UtcNow, clientId));
        }

    }
}