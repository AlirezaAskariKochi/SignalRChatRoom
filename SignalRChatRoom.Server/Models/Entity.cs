using System.ComponentModel.DataAnnotations;

namespace SignalRChatRoom.Server.Models
{
    public class Entity
    {
        [Key]
        public long Id { get;private set; }
        public DateTime CreateDate { get;private set; } = DateTime.UtcNow;
    }
}