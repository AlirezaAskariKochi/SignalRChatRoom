using SignalRChatRoom.Server.IRepositories;
using SignalRChatRoom.Server.Models;

namespace SignalRChatRoom.Server.Controllers
{
    public class ClientController : GenericController<Client>
    {
        public ClientController(IRepository<Client> clientRepository) : base(clientRepository) { }
    }
}
