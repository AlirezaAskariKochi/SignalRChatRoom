using System.ComponentModel.DataAnnotations;

namespace SignalRChatRoom.Server.Models.Enums;

public enum GroupType
{
    [Display(Name = "رویدادها")]
    Events = 0,

    [Display(Name = "کاری")]
    Work = 1
}