using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRChatRoom.Server.Models
{
    [Table("GroupClients", Schema = "Chats")]
    public class GroupClient 
    {
        public long ClientId { get; set; }
        public long GroupId { get; set; }
        public GroupClient()
        {
            
        }
    }
}
