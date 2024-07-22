using System.ComponentModel.DataAnnotations;

namespace SignalRChatRoom.Server.Models
{
    public enum MessageType
    {
        [Display(Name = "پیام عادی")]
        Normal=0,
        [Display(Name = "پیام محرمانه")]
        Secret =1,
    }
}