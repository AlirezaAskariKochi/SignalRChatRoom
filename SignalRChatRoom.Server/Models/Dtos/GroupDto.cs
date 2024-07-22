namespace SignalRChatRoom.Server.Models.Dtos
{
    public class GroupDto
    {
        public long Id { get; set; }
        public string GroupName { get; set; }
        public List<ClientDto> Clients { get; set; } = new List<ClientDto>();
    }
}
