using System.ComponentModel.DataAnnotations.Schema;
using SignalRChatRoom.Server.Models.Enums;

namespace SignalRChatRoom.Server.Models
{
    [Table("Groups", Schema = "Chats")]
    public class Group:Entity
    {
        public string GroupName { get;private set; }
        public GroupType GroupType { get; set; }
        public int? EntityType { get; set; }
        public long? EntityId { get; set; }
        public virtual ICollection<Client> Clients { get; } = new List<Client>();
        public virtual ICollection<ChatRoom> Messages { get; } = new List<ChatRoom>();
        public Group(string groupName)
        {
            GroupName = groupName;
        }
        public Group()
        {
            
        }
    }
}
