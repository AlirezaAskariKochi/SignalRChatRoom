using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalRChatRoom.Server.Models
{
    [Table("Clients", Schema = "Chats")]
    public class Client: Entity
    {

        [Required]
        [StringLength(50)]
        public string Guid { get;private set; }
        public string PersianName { get; private set; }
        public string ConnectionId { get; private set; }
        public string Username { get; private set; }
        public virtual ICollection<Group> Groups { get; } = new List<Group>();
        //public virtual ICollection<Client> Contacts { get; } = new List<Client>();
        public virtual ICollection<ChatRoom> SentMessages { get; } = new List<ChatRoom>();
        public virtual ICollection<ChatRoom> ReceivedMessages { get; } = new List<ChatRoom>();

        public Client(string guid, string persianName, string connectionId, string username)
        {
            Guid = guid;
            PersianName = persianName;
            ConnectionId = connectionId;
            Username = username;
        }
        public Client() {}
        public void AddConnectionId(string connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}
