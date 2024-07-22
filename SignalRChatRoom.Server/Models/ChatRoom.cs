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
        public MessageType MessageType { get; private set; }
        public long? ReplyId { get; private set; }
        public long? ForwardId { get; private set; }
        public bool Seen { get; private set; }
        public DateTime? SeenDateTime { get; private set; }
        public ICollection<SeenMessageLog> SeenMessageLogs { get;} = new List<SeenMessageLog>();
        public ICollection<Client> SecretClients { get;} = new List<Client>();
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
            MessageType = MessageType.Normal;
            ReplyId = replyId;
            ForwardId = forwardId;
            Seen = false;
        }
        public ChatRoom(long fromId, long? toId,long? groupId, string message, ChatType type, long? replyId, long? forwardId,List<Client> secretClients)
        {
            FromId = fromId;
            ToId = toId;
            GroupId = groupId;
            Message = message;
            Type = type;
            MessageType = MessageType.Secret;
            ReplyId = replyId;
            ForwardId = forwardId;
            Seen = false;
            SecretClients = secretClients;
        }
        public ChatRoom()
        {
                
        }

        public void AddSeenMessageLog(long clientId)
        {
            Seen=true;
            SeenDateTime=DateTime.UtcNow;
            SeenMessageLogs.Add(new SeenMessageLog(Id,clientId));
        }
    }
}