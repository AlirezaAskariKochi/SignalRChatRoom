using System.ComponentModel.DataAnnotations;

namespace SignalRChatRoom.Client.Models
{
    public enum MessageType
    {
        [Display(Name = "پیام عادی")]
        Normal = 0,
        [Display(Name = "پیام محرمانه")]
        Secret = 1,
    }
}